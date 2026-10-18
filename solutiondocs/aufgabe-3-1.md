# Lösungsvorschlag Aufgabe 3.1 – Datenmodell erweitern (.NET)

Branch: `block3/3-1-datenmodellErweitern`, analog zum Quarkus-Projekt.
Grundlage ist der Commit `8258e16` von Aufgabe 2.2 auf `block2/2-2-dto`.
Die Änderungen für Aufgabe 3.1 bleiben separat uncommittet.
.NET 10 und EF Core/Npgsql 10 bleiben unverändert.

## ERD

```mermaid
erDiagram
    CATEGORY o|--o{ ENTRY : kategorisiert
    ENTRY ||--o{ ENTRY_TAG : besitzt
    TAG ||--o{ ENTRY_TAG : zugeordnet
    CATEGORY {
        int Id PK
        text Title "NOT NULL"
    }
    ENTRY {
        int Id PK
        timestamptz CheckIn "NOT NULL"
        timestamptz CheckOut "NOT NULL"
        int CategoryId FK "optional"
    }
    TAG {
        int Id PK
        text Title "NOT NULL"
    }
    ENTRY_TAG {
        int EntryId PK,FK
        int TagId PK,FK
    }
```
