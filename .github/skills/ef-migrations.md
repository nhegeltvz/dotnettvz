# Skill: EF Migrations

Use this skill when the user asks to:
- Add a migration
- Update the database
- Apply EF changes
- Regenerate the schema

## Project structure

| Role | Project |
|------|---------|
| Startup project (connection string) | `Web` |
| DbContext + migrations | `Data` |
| DbContext class | `MatchTrackerDbContext` |

All `dotnet ef` commands must be run from the `Data/` directory.

## Step 1 – Ask for a migration name

Before running anything, ask the user:
> "What should the migration be named?"

Use a short PascalCase descriptive name, e.g. `AddPlayerBio`, `AddMatchVoteTable`.

## Step 2 – Add the migration

Run from the `Data/` directory:

```bash
dotnet ef migrations add <MigrationName> --startup-project ../Web --context MatchTrackerDbContext
```

## Step 3 – Apply to the database

```bash
dotnet ef database update --startup-project ../Web --context MatchTrackerDbContext
```

## Step 4 – Confirm

Tell the user:
- The migration file that was created (path under `Data/Migrations/`)
- That the database has been updated

## Notes

- If `dotnet ef` is not found, it needs to be installed: `dotnet tool install --global dotnet-ef`
- If the build fails before migration, fix the build error first — EF requires a successful build
- Never delete migration files manually; use `dotnet ef migrations remove` to undo the last one
