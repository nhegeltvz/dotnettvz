# Sitemap – MatchTracker URL Routing Model

## Home

| URL | Controller | Action | View |
|-----|-----------|--------|------|
| `/home` | `HomeController` | `Index` | `Views/Home/Index.cshtml` |
| `/home/index` | `HomeController` | `Index` | `Views/Home/Index.cshtml` |
| `/home/privacy` | `HomeController` | `Privacy` | `Views/Home/Privacy.cshtml` |
| `/home/error` | `HomeController` | `Error` | `Views/Shared/Error.cshtml` |

---

## Players

| URL | Controller | Action | View |
|-----|-----------|--------|------|
| `/players` | `PlayersController` | `Index` | `Views/Players/Players.cshtml` |
| `/players/list` | `PlayersController` | `Index` | `Views/Players/Players.cshtml` |
| `/players/details/{id:guid}` | `PlayersController` | `Details` | `Views/Players/PlayerDetailsView.cshtml` |
| `/players/details/by-username/{username}` | `PlayersController` | `Details` | `Views/Players/PlayerDetailsView.cshtml` |

**Partial views used:** `Views/Shared/_PlayerCard.cshtml`

---

## Matches

| URL | Controller | Action | View |
|-----|-----------|--------|------|
| `/matches` | `MatchesController` | `Index` | `Views/Matches/MatchesView.cshtml` |
| `/matches/list` | `MatchesController` | `Index` | `Views/Matches/MatchesView.cshtml` |
| `/matches/details/{id:guid}` | `MatchesController` | `Details` | `Views/Matches/MatchDetailsView.cshtml` |
| `/matches/details/by-date/{matchDate}/{fieldName}` | `MatchesController` | `Details` | `Views/Matches/MatchDetailsView.cshtml` |

> `matchDate` format: `yyyy-MM-dd`

**Partial views used:** `Views/Shared/_MatchCard.cshtml`

---

## Parties

| URL | Controller | Action | View |
|-----|-----------|--------|------|
| `/parties` | `PartiesController` | `Index` | `Views/Parties/PartiesView.cshtml` |
| `/parties/list` | `PartiesController` | `Index` | `Views/Parties/PartiesView.cshtml` |
| `/parties/details/{id:guid}` | `PartiesController` | `Details` | `Views/Parties/PartyDetailsView.cshtml` |
| `/parties/details/by-date/{createdAt}` | `PartiesController` | `Details` | `Views/Parties/PartyDetailsView.cshtml` |

**Partial views used:** `Views/Shared/_PartyCard.cshtml`

---

## Stadiums

| URL | Controller | Action | View |
|-----|-----------|--------|------|
| `/stadiums` | `StadiumsController` | `Index` | `Views/Stadiums/StadiumsView.cshtml` |
| `/stadiums/list` | `StadiumsController` | `Index` | `Views/Stadiums/StadiumsView.cshtml` |
| `/stadiums/details/{id:guid}` | `StadiumsController` | `Details` | `Views/Stadiums/StadiumDetailsView.cshtml` |
| `/stadiums/details/by-name/{name}` | `StadiumsController` | `Details` | `Views/Stadiums/StadiumDetailsView.cshtml` |

**Partial views used:** `Views/Shared/_StadiumCard.cshtml`

---

## Shared Layout & Partials

| File | Purpose |
|------|---------|
| `Views/Shared/_Layout.cshtml` | Master layout used by all views |
| `Views/Shared/_Breadcrumbs.cshtml` | Breadcrumb navigation partial |
| `Views/Shared/_PlayerCard.cshtml` | Reusable player card component |
| `Views/Shared/_MatchCard.cshtml` | Reusable match card component |
| `Views/Shared/_PartyCard.cshtml` | Reusable party card component |
| `Views/Shared/_StadiumCard.cshtml` | Reusable stadium card component |
| `Views/Shared/Error.cshtml` | Generic error page |
| `Views/Shared/_ValidationScriptsPartial.cshtml` | Client-side validation scripts |
