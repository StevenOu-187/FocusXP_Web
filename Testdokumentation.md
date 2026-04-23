# Testdokumentation – FocusXP / LearningManager

**Projekt:** FocusXP – LearningManager Web  
**Datum:** 15.04.2026

---

## 1 Testspezifikation

> Basis: Unit-Tests des `LearningManager.Test`-Projekts.  
> Testtyp: Automatisierte Unit-Tests mit MSTest 4 + EF Core InMemory.  
> Jeder Testfall entspricht einer Service-Methode bzw. einem Verhalten aus den Use Cases.

---

### SCH01 – Keine Lernblöcke vorhanden → leere Planung

| #   | Schritt                                                            | Erwartetes Ergebnis                 |
| --- | ------------------------------------------------------------------ | ----------------------------------- |
| 1   | In-Memory-DB aufsetzen (kein LearningSlot, ein TaskItem vorhanden) | DB ist leer bis auf den einen Task  |
| 2   | `SchedulingService.GetScheduleAsync(today, today+6)` aufrufen      | Methode kehrt ohne Exception zurück |
| 3   | Rückgabewert prüfen                                                | Leere Liste (Count = 0)             |

---

### SCH02 – Keine Tasks vorhanden → leere Planung

| #   | Schritt                                                  | Erwartetes Ergebnis                 |
| --- | -------------------------------------------------------- | ----------------------------------- |
| 1   | In-Memory-DB aufsetzen (ein LearningSlot, kein TaskItem) | DB enthält nur den Slot             |
| 2   | `GetScheduleAsync(today, today+6)` aufrufen              | Methode kehrt ohne Exception zurück |
| 3   | Rückgabewert prüfen                                      | Leere Liste (Count = 0)             |

---

### SCH03 – Ungültige Range: `from > to` → leere Planung

| #   | Schritt                                                            | Erwartetes Ergebnis                 |
| --- | ------------------------------------------------------------------ | ----------------------------------- |
| 1   | In-Memory-DB aufsetzen (leer)                                      | DB ist leer                         |
| 2   | `GetScheduleAsync(today+5, today+1)` aufrufen (from liegt nach to) | Methode kehrt ohne Exception zurück |
| 3   | Rückgabewert prüfen                                                | Leere Liste (Count = 0)             |

---

### SCH04 – Alle Tasks haben Status „Done" → leere Planung

| #   | Schritt                                                           | Erwartetes Ergebnis                           |
| --- | ----------------------------------------------------------------- | --------------------------------------------- |
| 1   | Einen LearningSlot und einen TaskItem mit `Status = Done` anlegen | Daten sind gespeichert                        |
| 2   | `GetScheduleAsync(today, today+6)` aufrufen                       | Methode kehrt ohne Exception zurück           |
| 3   | Rückgabewert prüfen                                               | Leere Liste, weil Done-Tasks ignoriert werden |

---

### SCH05 – Task passt exakt in einen Slot

| #   | Schritt                                               | Erwartetes Ergebnis                 |
| --- | ----------------------------------------------------- | ----------------------------------- |
| 1   | Slot anlegen: heute, 08:00–10:00 (2 h)                | Slot gespeichert                    |
| 2   | Task anlegen: `EstimatedHours = 2`, DueDate = heute+7 | Task gespeichert                    |
| 3   | `GetScheduleAsync(today, today)` aufrufen             | Methode kehrt ohne Exception zurück |
| 4   | Anzahl der Einträge prüfen                            | Genau 1 Eintrag                     |
| 5   | Start- und Endzeit des Eintrags prüfen                | StartTime = 08:00, EndTime = 10:00  |

---

### SCH06 – Task größer als ein Slot → verteilt auf zwei Slots

| #   | Schritt                                               | Erwartetes Ergebnis                        |
| --- | ----------------------------------------------------- | ------------------------------------------ |
| 1   | Slot 1 anlegen: heute, 08:00–10:00 (2 h)              | Slot gespeichert                           |
| 2   | Slot 2 anlegen: morgen, 08:00–10:00 (2 h)             | Slot gespeichert                           |
| 3   | Task anlegen: `EstimatedHours = 3`, DueDate = heute+7 | Task gespeichert                           |
| 4   | `GetScheduleAsync(today, today+1)` aufrufen           | Methode kehrt ohne Exception zurück        |
| 5   | Anzahl der Einträge prüfen                            | Genau 2 Einträge (heute: 2 h, morgen: 1 h) |
| 6   | Titel beider Einträge prüfen                          | Beide Einträge gehören zum selben Task     |

---

### SCH07 – Mehrere Tasks werden nahtlos in einem Slot verteilt

| #   | Schritt                                                 | Erwartetes Ergebnis                        |
| --- | ------------------------------------------------------- | ------------------------------------------ |
| 1   | Slot anlegen: heute, 08:00–12:00 (4 h)                  | Slot gespeichert                           |
| 2   | Task 1 anlegen: `EstimatedHours = 2`, DueDate = heute+3 | Früherer DueDate → höhere Priorität        |
| 3   | Task 2 anlegen: `EstimatedHours = 2`, DueDate = heute+5 | Späterer DueDate → niedrigere Priorität    |
| 4   | `GetScheduleAsync(today, today)` aufrufen               | Methode kehrt ohne Exception zurück        |
| 5   | Reihenfolge der Einträge prüfen                         | Task 1 vor Task 2                          |
| 6   | Übergänge prüfen                                        | `EndTime(Eintrag1) == StartTime(Eintrag2)` |

---

### SCH08 – Slot mit 0 Stunden Dauer wird übersprungen

| #   | Schritt                                                  | Erwartetes Ergebnis                                   |
| --- | -------------------------------------------------------- | ----------------------------------------------------- |
| 1   | Slot anlegen: heute, `StartTime = EndTime = 09:00` (0 h) | Slot gespeichert (kein Validierungsfehler im Service) |
| 2   | Task anlegen: `EstimatedHours = 1`                       | Task gespeichert                                      |
| 3   | `GetScheduleAsync(today, today)` aufrufen                | Methode kehrt ohne Exception zurück                   |
| 4   | Rückgabewert prüfen                                      | Leere Liste, weil der Slot keine Kapazität hat        |

---

### SCH09 – Kapazität erschöpft vor Zuteilung aller Tasks

| #   | Schritt                                                 | Erwartetes Ergebnis                 |
| --- | ------------------------------------------------------- | ----------------------------------- |
| 1   | Slot anlegen: heute, 09:00–10:00 (1 h)                  | Slot gespeichert                    |
| 2   | Task 1 anlegen: `EstimatedHours = 1`, DueDate = heute+2 | Passt genau in den Slot             |
| 3   | Task 2 anlegen: `EstimatedHours = 2`, DueDate = heute+3 | Kein Platz mehr                     |
| 4   | `GetScheduleAsync(today, today)` aufrufen               | Methode kehrt ohne Exception zurück |
| 5   | Ergebnis prüfen                                         | Nur Task 1 ist eingeplant           |

---

### SCH10 – Task mit vergangenem DueDate wird übersprungen

| #   | Schritt                                            | Erwartetes Ergebnis                       |
| --- | -------------------------------------------------- | ----------------------------------------- |
| 1   | Slot anlegen: heute, 09:00–11:00 (2 h)             | Slot gespeichert                          |
| 2   | Task anlegen: `DueDate = gestern`, `Status = Open` | Task mit abgelaufenem Datum gespeichert   |
| 3   | `GetScheduleAsync(today, today)` aufrufen          | Methode kehrt ohne Exception zurück       |
| 4   | Rückgabewert prüfen                                | Leere Liste – abgelaufener Task ignoriert |

---

### SCH11 – Gesperrter Tag wird nicht eingeplant

| #   | Schritt                                               | Erwartetes Ergebnis                         |
| --- | ----------------------------------------------------- | ------------------------------------------- |
| 1   | Slot anlegen: heute                                   | Slot gespeichert                            |
| 2   | Task anlegen: `EstimatedHours = 2`, DueDate = heute+7 | Task gespeichert                            |
| 3   | BlockedDay für heute anlegen: `Reason = "Feiertag"`   | BlockedDay gespeichert                      |
| 4   | `GetScheduleAsync(today, today)` aufrufen             | Methode kehrt ohne Exception zurück         |
| 5   | Rückgabewert prüfen                                   | Leere Liste – gesperrter Tag wird ignoriert |

---

### SCH12 – InProgress-Task hat Priorität über Open-Task bei gleichem DueDate

| #   | Schritt                                                                 | Erwartetes Ergebnis                    |
| --- | ----------------------------------------------------------------------- | -------------------------------------- |
| 1   | Slot anlegen: heute, 09:00–10:00 (1 h – nur Platz für 1 Task)           | Slot gespeichert                       |
| 2   | Task A anlegen: `Status = Open`, DueDate = heute+5 (zuerst gespeichert) | Task A gespeichert                     |
| 3   | Task B anlegen: `Status = InProgress`, DueDate = heute+5                | Task B gespeichert                     |
| 4   | `GetScheduleAsync(today, today)` aufrufen                               | Methode kehrt ohne Exception zurück    |
| 5   | Ergebnis prüfen                                                         | Nur Task B (InProgress) ist eingeplant |

---

### SCH13 – Vergangener Tag wird nicht eingeplant

| #   | Schritt                                     | Erwartetes Ergebnis                 |
| --- | ------------------------------------------- | ----------------------------------- |
| 1   | Slot für gestern und heute anlegen          | Beide Slots gespeichert             |
| 2   | Task anlegen: DueDate = heute+7             | Task gespeichert                    |
| 3   | `GetScheduleAsync(gestern, heute)` aufrufen | Methode kehrt ohne Exception zurück |
| 4   | Alle Einträge prüfen                        | Kein Eintrag hat `Date < heute`     |

---

### SLT01 – StartTime >= EndTime → ValidationException

| #   | Schritt                                                                                     | Erwartetes Ergebnis                             |
| --- | ------------------------------------------------------------------------------------------- | ----------------------------------------------- |
| 1   | `LearningSlotService.CreateAsync(slot)` mit `StartTime = 11:00`, `EndTime = 09:00` aufrufen | `LearningSlotValidationException` wird geworfen |
| 2   | Dasselbe mit `StartTime = EndTime = 10:00`                                                  | `LearningSlotValidationException` wird geworfen |

---

### SLT02 – Überlappender Slot am selben Tag → ValidationException

| #   | Schritt                                             | Erwartetes Ergebnis                             |
| --- | --------------------------------------------------- | ----------------------------------------------- |
| 1   | Ersten Slot anlegen: Montag, 08:00–10:00            | Slot erfolgreich gespeichert                    |
| 2   | Zweiten Slot anlegen: Montag, 09:00–11:00 (Overlap) | `LearningSlotValidationException` wird geworfen |

---

### SLT03 – Nicht-überlappender Slot am selben Tag → Erfolg

| #   | Schritt                                                             | Erwartetes Ergebnis                       |
| --- | ------------------------------------------------------------------- | ----------------------------------------- |
| 1   | Ersten Slot anlegen: Donnerstag, 08:00–10:00                        | Slot erfolgreich gespeichert              |
| 2   | Zweiten Slot anlegen: Donnerstag, 10:00–12:00 (direkt anschließend) | Slot erfolgreich gespeichert, kein Fehler |
| 3   | StartTime des zweiten Slots prüfen                                  | `StartTime = 10:00`                       |

---

### TSK01 – Task erstellen und speichern

| #   | Schritt                                                                         | Erwartetes Ergebnis                          |
| --- | ------------------------------------------------------------------------------- | -------------------------------------------- |
| 1   | `TaskItemService.CreateAsync(item)` mit Titel, DueDate, EstimatedHours aufrufen | Methode kehrt ohne Exception zurück          |
| 2   | Zurückgegebene ID prüfen                                                        | `Id > 0` (Datenbankschlüssel wurde vergeben) |
| 3   | Zurückgegebenen Titel prüfen                                                    | Titel stimmt mit Eingabe überein             |

---

### TSK02 – Nicht-existierende Task-ID aktualisieren → null

| #   | Schritt                                                  | Erwartetes Ergebnis                 |
| --- | -------------------------------------------------------- | ----------------------------------- |
| 1   | `TaskItemService.UpdateAsync(999, updatedItem)` aufrufen | Methode kehrt ohne Exception zurück |
| 2   | Rückgabewert prüfen                                      | `null` (ID existiert nicht)         |

---

### BLK01 – Bereits gesperrtes Datum → Reason wird aktualisiert, kein Duplikat

| #   | Schritt                                                                              | Erwartetes Ergebnis        |
| --- | ------------------------------------------------------------------------------------ | -------------------------- |
| 1   | `BlockedDayService.CreateAsync` mit Datum 01.01.2025, Reason „Erster Grund" aufrufen | BlockedDay gespeichert     |
| 2   | `CreateAsync` mit demselben Datum, Reason „Neujahr" aufrufen                         | Kein Fehler, kein Duplikat |
| 3   | Reason des zurückgegebenen Objekts prüfen                                            | `Reason = "Neujahr"`       |
| 4   | Alle Einträge für dieses Datum zählen                                                | Genau 1 Eintrag            |

---

### BLK02 – Nicht-existierendes Datum löschen → false

| #   | Schritt                                                                  | Erwartetes Ergebnis                 |
| --- | ------------------------------------------------------------------------ | ----------------------------------- |
| 1   | `BlockedDayService.DeleteByDateAsync(new DateOnly(2099, 1, 1))` aufrufen | Methode kehrt ohne Exception zurück |
| 2   | Rückgabewert prüfen                                                      | `false` (Datum existiert nicht)     |

---

---

## 2 Testprotokoll

**Testlauf am:** 15.04.2026  
**Umgebung:** Windows 11 Pro, .NET 10.0, MSTest 4.0.2, EF Core InMemory 10.0.3  
**Ausführung:** `dotnet test LearningManager.Test/LearningManager.Test.csproj`  
**Gesamtdauer:** 640 ms

| TC-ID | Testname                                                                                                                   | Ergebnis   | Kommentar bei Fehler |
| ----- | -------------------------------------------------------------------------------------------------------------------------- | ---------- | -------------------- |
| SCH01 | NoSlots_ReturnsEmptyList                                                                                                   | **passed** |                      |
| SCH02 | NoTasks_ReturnsEmptyList                                                                                                   | **passed** |                      |
| SCH03 | FromGreaterThanTo_ReturnsEmptyList                                                                                         | **passed** |                      |
| SCH04 | AllTasksDone_ReturnsEmptyList                                                                                              | **passed** |                      |
| SCH05 | SingleTask_FitsExactlyInSlot_ProducesOneEntry                                                                              | **passed** |                      |
| SCH06 | SingleTask_LargerThanOneSlot_SpansAcrossTwoSlots                                                                           | **passed** |                      |
| SCH07 | MultipleTasksDistributedAcrossSlots                                                                                        | **passed** |                      |
| SCH08 | SlotWithZeroDuration_IsSkipped                                                                                             | **passed** |                      |
| SCH09 | AllSlotsConsumed_BeforeAllTasksAssigned_RemainingTasksNotInResult                                                          | **passed** |                      |
| SCH10 | TaskWithPastDueDate_IsSkipped                                                                                              | **passed** |                      |
| SCH11 | BlockedDay_ProducesNoEntriesForThatDay                                                                                     | **passed** |                      |
| SCH12 | InProgressTask_PrioritizedOverOpenTask_WithSameDueDate                                                                     | **passed** |                      |
| SCH13 | PastDay_IsNotScheduled                                                                                                     | **passed** |                      |
| SLT01 | CreateAsync_StartTimeAfterEndTime_ThrowsValidationException / CreateAsync_StartTimeEqualsEndTime_ThrowsValidationException | **passed** |                      |
| SLT02 | CreateAsync_OverlappingSlot_SameDay_ThrowsValidationException                                                              | **passed** |                      |
| SLT03 | CreateAsync_NonOverlappingSlot_SameDay_Succeeds                                                                            | **passed** |                      |
| TSK01 | CreateAsync_SavesItemAndReturnsWithId                                                                                      | **passed** |                      |
| TSK02 | UpdateAsync_NonExistentId_ReturnsNull                                                                                      | **passed** |                      |
| BLK01 | CreateAsync_ExistingDate_UpdatesReasonInsteadOfDuplicate                                                                   | **passed** |                      |
| BLK02 | DeleteByDateAsync_NonExistentDate_ReturnsFalse                                                                             | **passed** |                      |

**Zusammenfassung:**

|            | Anzahl |
| ---------- | ------ |
| passed     | 20     |
| failed     | 0      |
| blocked    | 0      |
| **gesamt** | **20** |

Alle 20 spezifizierten Testfälle wurden erfolgreich ausgeführt.  
Der Testlauf zeigt, dass die Kernlogik des `SchedulingService` (Algorithmus, Filterung, Edge Cases) sowie alle CRUD-Services (`LearningSlotService`, `TaskItemService`, `BlockedDayService`) korrekt funktionieren.
