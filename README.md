# Foodtrucker

---

### System interakcji między obsługą foodtrucków a klientami

### Mirosław Gałczyński

---

## 1. Model bazy danych

### 1.1 Model logiczny bazy danych

![Diagram](Diagrams/Model_bazy_danych.jpg?raw=true "Model bazy danych")

### 1.2 Opis tabel

Tabela Users
Tabela użytkowników będzie zawierała informacje na ich temat, będzie rozszerzeniem tabeli wymaganej przez framework ASP.NET Core ([Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<TKey\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identityuser-1)).
Będą trzy typy użytkowników:

1. FoodtruckStaff – pracownicy foodtrucków
2. ServiceStaff – pracownicy obsługi systemu (większość funkcjonalności zostanie dodane później
3. Customer – klienci foodtrucków

Typy będą trzymane przez: [Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<TKey\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identityrole-1)

#### Tabela Foodtrucks

Tabela ta będzie zawierała informacje na temat foodtrucków (nazwy, domyślne lokalizacje, godziny otwarcia), kluczem głównym tej tabeli będzie klucz sztuczny typu GUID.

#### Tabela Presences

Tabela ta będzie zawierała informacje na temat obecności foodtrucków poza ich domyślną lokalizacją (będzie zawierała informacje kiedy i gdzie foodtruck będzie obecny), kluczem głównym tej tabeli będzie klucz sztuczny typu GUID.

#### Tabela Abscensces

Tabela ta będzie zawierała informacje na temat niedostępności foodtrucka w godzinach pracy w żadnym miejscu.

#### Tabela FoodtruckOwnership

Tabela ta będzie zawierała informacje na temat posiadania (różne klasy uprawnień – kolumna type) foodtrucków. Kluczem głównym będzie klucz złożony z kluczy obcych (Users i Foodtruck).

#### Tabela FavoriteFoodtrucks

Tabela będzie przechowywała informacje które Foodtrucki użytkownik typu Customer zapisał jako ulubione. Kluczem głównym będzie klucz złożony z kluczy obcych (Users i Foodtruck).

## Diagram przejść

![Diagram](Diagrams/Diagram_przejsc_klienci.jpg?raw=true "Diagram przejść dla klientów foodtrucków")
![Diagram](Diagrams/Diagram_przejsc_pracownicy_foodtruckow.jpg?raw=true "Diagram przejść dla pracowników foodtrucków foodtrucków")

## Wybrane technologie i narzędzia

| Klasa narzędzia          | Wybrane narzędzie                                                   |
| ------------------------ | ------------------------------------------------------------------- |
| Framwork webowy          | ASP.NET Core                                                        |
| ORM                      | Entity Framework Core z Npgsql i NetTopologySuite (obsługa Spatial) |
| Framework JS             | React z Redux                                                       |
| IDE                      | JetBreins Rider                                                     |
| VCS                      | GIT                                                                 |
| Hosting repozytorium git | BitBucket                                                           |

## Opis architektury aplikacji dla warstwy serverside

W przypadku wszystkich domen działania aplikacji z wyjątkiem uwierzytelniania aplikacja będzie dwuwarstowa Gdzie jedną warstwą będą serwisy dające warstwę abstrakcji bazie danych i operujące wystawiające interejsy z obiektami `Dto`. Drugą warstwą będą kontrolery wystawiające rest api. W przypadku uwierzytelniania biblioteka udostępnia już interfejs który używa objektów encji.
