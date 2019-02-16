# Foodtrucker
## Dokumentacja powykonawcza
### Zakres applikacji wykonany podczas realizacji projektu
Podczas realizacji kursu został zrealizowany mniejszy niż zostało to przewidziane. Zrealizowane przypadki użycia można podzielić na przypadki użycia dla klientów foodtrucków i dla pracowników foodtrucków.
#### Zrealizowane przypadki użycia dostępnych dla klientów foodtrucków
![Diagram](Diagrams/Diagram_przejsc_klienci.jpg?raw=true "Zaplanowane przypadki użycia klientów foodtrucków")
Zrealizowane zostały wszystkie przypadki użycia z wyjątkiem zarządzania ulubionymi subskrybcjami. Dodatkowo podczas wytwarzania aplikacji okazało się konieczne roszrzenie przypadku użycia --Przeglądanie foodtrucków-- do --Przeglądanie foodtrucków oraz obecności-- oraz --Przeglądanie foodtrukców, będąc zalogowanych-- do --Przeglądanie foodtrucków oraz obecności, będąc zalogowanym--.
//screnshoty
#### Zrealizowane przypadki użycia dostępnych dla pracowników foodtrucków
![Diagram](Diagrams/Diagram_przejsc_pracownicy_foodtruckow.jpg?raw=true "Zaplanowane przypadki użycia klientów foodtrucków")
Zrealizowane zostały wszystkie przypadki użycia z wyjątkiem przypadków dotycących zarządania nieobecnościami.
//screenshoty
### Uwagi techniczne
Usługa BitBucket została zmieniona na GitHub.
Do technologi użytych podczas wytwarzania aplikacji dołączył Jenkins - otwartoźródłowa usługa ulatwiająca automatyzacje zadań. W projekcie został wykorzystany do automatyzacji testów oraz automatyzacji publikowania oprogramowania na serwerze testowym. Akcje są inicjowane po opublikowaniu commitu do repozytorium GitHub.
#### Leaflet
Podczas realizacji aplikacji wynikła konieczność integracji biblioteki wspomagającej wyświetlanie Leaflet z React, konieczność ta wynikała z lepszego bilansu korzyści w porówanianiu z kosztami niż dało by to gotowe rozwiązanie.
Integracje tą można 
//screenshoty