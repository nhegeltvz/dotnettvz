# Sitemap – MatchTracker URL Routing Model

This sitemap is derived from the breadcrumbs defined in Razor views and the current controller routing.

## Public routes

| URL                      | Controller           | Action    | View                                 | Breadcrumbs                         |
| ------------------------ | -------------------- | --------- | ------------------------------------ | ----------------------------------- |
| `/`                      | `HomeController`     | `Index`   | `Home/Index.cshtml`                  | (no breadcrumbs)                    |
| `/home`                  | `HomeController`     | `Index`   | `Home/Index.cshtml`                  | (no breadcrumbs)                    |
| `/privacy`               | `HomeController`     | `Privacy` | `Home/Privacy.cshtml`                | (no breadcrumbs)                    |
| `/error`                 | `HomeController`     | `Error`   | `Shared/Error.cshtml`                | (no breadcrumbs)                    |
| `/matches`               | `MatchesController`  | `Index`   | `Matches/MatchesView.cshtml`         | Home > Matches                      |
| `/matches/details/{id}`  | `MatchesController`  | `Details` | `Matches/MatchDetailsView.cshtml`    | Home > Matches > {FieldName — Date} |
| `/parties`               | `PartiesController`  | `Index`   | `Parties/PartiesView.cshtml`         | Home > Parties                      |
| `/parties/details/{id}`  | `PartiesController`  | `Details` | `Parties/PartyDetailsView.cshtml`    | Home > Parties > {PartyDescription} |
| `/players`               | `PlayersController`  | `Index`   | `Players/Players.cshtml`             | Home > Players                      |
| `/players/details/{id}`  | `PlayersController`  | `Details` | `Players/PlayerDetailsView.cshtml`   | Home > Players > {Username}         |
| `/stadiums`              | `StadiumsController` | `Index`   | `Stadiums/StadiumsView.cshtml`       | Home > Stadiums                     |
| `/stadiums/details/{id}` | `StadiumsController` | `Details` | `Stadiums/StadiumDetailsView.cshtml` | Home > Stadiums > {FieldName}       |

## Notes

- The default conventional route also allows `/Home/Index`, `/Matches/Index`, etc., but the UI breadcrumbs and navigation links use the lowercase path variants shown above.
- `{id}` is a GUID route parameter for details pages.
