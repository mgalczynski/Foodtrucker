# Foodtrucker
### System interakcji między obsługą foodtrucków a klientami
### Mirosław Gałczyński

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

## Wybrane technologie i narzędzia
Jako główną technologię został wybrany ASP.NET Core oraz Entity Framework Core z Ngpsql (determinowany przez wybór PostgreSQL jako bazy danych) do persystencji danych oraz React z Reduxem w warstwie prezentacji. Jako edytor został wybrany JetBrains Rider. Do hostowania repozytorium został wybrany BitBucket.