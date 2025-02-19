# Aquila Game
<p align="justify">Przedstawiony projekt to gra komputerowa, której głównym założeniem było wykorzystanie elementów losowości w wielu aspektach rozgrywki. Projekt ten został zrealizowany przez trzech autorów:</p>
     
      Jakub Kopeć
      Paweł Pieczykolan
      Marek Prokopiuk
---
### Instrukcja uruchomienia projektu pracy

1. Należy pobrać Unity Hub ze strony https://unity.com/download
2. Po pobraniu uruchomić Unity Hub
3. W Unity Hub otworzyć zakładkę "Installs"
4. W prawym górnym rogu kliknąć "Install Editor" w celu zainstalowania odpowiedniej wersji edytora Unity
5. Wybrać zakładkę "Archive", i tam kliknąć "download archive", powinna otworzyć się strona https://unity.com/releases/editor/archive
6. Wersja edytora użyta w projekcie to 2022.3.33f1, więc na stronie należy wybrać zakładkę "2022", następnie "LTS(Default)" i odnaleźć na liście odpowiednią wersję
7. Po znalezieniu wersji edytora Unity 2022.3.33f1, należy w kolumnie "Hub installation" kliknąć "INSTALL"
8. Otworzy się okienko w przeglądarce z pytaniem "Otworzyć Unity Hub?", należy wtedy kliknąć "Otwórz Unity Hub" i powinno uruchomić się Unity Hub
9. W Unity Hub otworzy się okno instalacji wybranej wersji edytora Unity, należy wtedy klikąć "Install"
10. Po zainstalowaniu należy przenieść folder z projektem pracy o nazwie "Projekt Pracy" na dysk komputera (wymagane jest 10 GB wolnego miejsca)
11. Następnie w zakładce "Projects" programu Unity Hub rozwinąć przycisk "Add", który jest w prawym górnym rogu i wybrać opcję "Add project from disk"
12. Potem należy wybrać lokalizację folderu z plikami projektu, który został utworzony w punkcie 10
13. W zakładce "Projects" pojawi się nowy projekt o nazwie wybranego folderu, należy wtedy wybrać odpowiednią wersję edytora Unity (2022.3.33f1) i uruchomić projekt
14. Proszę przygotować kawę lub herbatę, gdyż proces uruchamiania projektu pierwszy raz może zająć całkiem długo
15. Po dłuższym czasie uruchomi się okno edytora Unity, aby włączyć grę w trybie Play Edytora należy otworzyć zakładkę "Project"
16. Następnie w folderze Assets/Scenes/Interfaces należy kliknąć dwukrotnie scenę o nazwie "Premenu"
17. Otworzy się podstawowa scena tego projektu, po kliknięciu przycisku "Play" (strzałka na środku górnego paska edytora) uruchomi się gra od początkowego momentu, co umożliwi przetestowanie rozgrywki w przewidzianym przez twórców cyklu

<p align="justify">W przypadku wystąpienia problemów, które nie zostały tu opisane lub braku opisanych funkcjonalności spowodowanym aktualizacjami oprogramowania, należy skorzystać z zasobów internetowych do znalezienia rozwiązania.</p>

---

### Opis projektu

<p align="justify">Gra zawiera wiele losowo generowanych elementów takich jak wyspy, obiekty, ścieżki, przeciwnicy oraz misje poboczne. Tematyką gry jest złoty okres piractwa XVII wieku. Rozgrywka jest widziana z perspektywy kamery izometrycznej, a gra utrzymana jest w kreskówkowej stylistyce. Możliwe jest sterowanie jedną postacią pirata na wyspie, bądź statkiem na oceanie, w zależności od aktualnej sytuacji w grze, eksploracja świata gry poprzez podróżowanie pomiędzy wyspami i odkrywanie nowych lokacji i zasobów, a także interakcja z określonymi obiektami takimi jak skrzynia. Gra zapewnia wgląd we wszystkie plansze menu takie jak menu główne, opcje, sterowanie, ekwipunek, menu pauzy i mapę, mechanizmy rozwoju postaci i systemu walki oraz możliwość zapisywania i wczytywania postępów gry.</p> 

<p align="justify">Do głównych narzędzi i technologii wykorzystanych w projekcie należą:</p>

  - <p align="justify">Unity – Silnik do gier wykorzystany do stworzenia świata gry, implementacji mechanizmów walki, rozgrywki oraz fizyki, a także zarządzania obiektami</p>
  - <p align="justify">Blender – Narzędzie do grafiki trójwymiarowej, wykorzystane do stworzenia wszelkich obiektów dostępnych w grze, a także do zrealizowania animacji gracza bądź otwierania skrzyni</p>
  - <p align="justify">Substance Painter – Oprogramowanie przeznaczone do teksturowania modeli 3D</p>
  - <p align="justify">Adobe XD – Narzędzie projektowe użyte do zaprojektowania wszystkich interfejsów menu, rozgrywki i gracza</p>
  - <p align="justify">JSON – Lekki format danych wykorzystany do zrealizowania mechanizmów zapisywania i wczytywania gry, czyli przechowywania informacji między innymi o wygenerowanych wyspach, obiektach i zdobytych przez gracza statystykach</p>
  - <p align="justify">Plastic SCM – System kontroli wersji wykorzytany w pracy do skutecznego zarządzania wersjami projektu głównego w silniku Unity</p>
  
<p align="justify">Wszystkie elementy wykorzystane w grze, takie jak obiekty, interfejsy, czy muzyka, są stworzone samodzielnie od podstaw przez autorów projektu. Do najważniejszych autorskich obiektów zaliczają się postać gracza, statek, przeciwnicy, szabla, miniatury wysp oraz obiekty generowane na grywalnych scenach wysp, czyli między innymi drzewa, palmy, krzaki, kaktusy, skały, beczki, czy skrzynie.</p>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/AutorskieObiekty.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 1. Autorskie obiekty generowane na wyspach</i>
</p>

<p align="justify">Po rozpoczęciu przez gracza nowej gry z menu głównego uruchamia się scena oceanu, na której generują się miniatury wysp. Mają one trzy wielkości – mała, średnia oraz duża, a także trzy typy, czyli piaszczysta, błotnista i trawiasta. Każda wyspa należy również do jednej z trzech istniejących w grze pirackich frakcji. Miniwyspy są generowane w losowych położeniach i rotacjach, zgodnych z określonymi warunkami: muszą znajdować się w wyznaczonym obszarze, a ich odległość od innych wysp nie może być zbyt mała, aby nie kolidowały ze sobą. Widoczne miniwyspy odzwierciedlają sceny tych wysp, zgodnie z typem i wielkością, na które można przejść poprzez interakcję statku gracza z miniwyspą.</p>

<p align="justify">Gracz porusza się po oceanie statkiem. Interfejs na tej scenie zawiera minimapę, z widoczną ikonką gracza, a także informacje dotyczące interakcji z miniwyspą, które pojawiają się, gdy gracz podpłynie wystarczająco blisko danej wyspy. Informacje te obejmują wielkość, typ oraz przynależność wyspy do frakcji. Po naciśnięciu przycisku interakcji uruchamia się scena odpowiadająca właściwej wyspie.</p>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/GenerowanieLosoweWyspy.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 2. Generowanie losowe - wyspy</i>
</p><br>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/RozgrywkaWyspy.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 3. Widok interfejsu gracza na oceanie</i>
</p>

<p align="justify">Po wejściu na wybraną wyspę generowana jest ścieżka wraz z odnogami oraz obiekty odpowiadające typowi wyspy. W celu wygenerowania ścieżki losowane są kolejne jej punkty pod odpowiednimi warunkami (np. omijanie pagórków i nieprawidłowego nachylenia terenu), a następnie na całość nakładana jest zależna od typu wyspy tekstura. Odnogi ścieżki również generowane są losowo – ich liczba oraz punkty początkowe są dobierane dynamicznie, z uwzględnieniem odpowiednich warunków. Tekstura ścieżki płynnie przechodzi w teksturę główną terenu, co zapewnia estetyczny efekt wizualny.</p>

<p align="justify">Obiekty na wyspach generowane są zgodnie z ich typem, czyli np. palmy i kaktusy na piaszczystej, a drzewa i krzaki na trawiastej. Obiekty generują się w odpowiednich odległościach od siebie, a także od ścieżek i innych statycznych elementów wysp. Dostosowują się do nachylenia terenu, ale też nie mogą się wygenerować na terenie nieodpowiednim, czyli o nieprawidłowej wysokości, bądź zbyt dużym nachyleniu. Na mapach generują się również przeciwnicy w zależności od frakcji. Poruszają się oni w losowych kierunkach, omijają przeszkody, a gdy wykryją gracza wystarczająco blisko siebie, zaczynają go gonić, a następnie atakować. Każdy przeciwnik ma losowe statystyki generowane z zakresu przygotowanego na podstawie aktualnych statystyk gracza w taki sposób, że powstało sześć poziomów trudności przeciwników (Na Rys. 5. widoczny jest pirat o najtrudniejszym poziomie trudności co dodatkowo oznaczono odpowiednią ikoną).</p>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/GenerowanieLosoweŚcieżka.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 4. Generowanie losowe - ścieżka</i>
</p><br>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/GenerowanieLosoweObiekty.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 5. Generowanie losowe - obiekty i przeciwnicy</i>
</p>

<p align="justify">Gracz po wyspach porusza się postacią pirata. Interfejs na tych scenach zawiera minimapę, na której widoczna jest zarówno ikonka gracza, jak i ikonki najbliższych przeciwników. Dostępne jest menu akcji z widocznym poziomem gracza, paskiem doświadczenia oraz zdrowia, a także akcjami szybkiego dostępu np. akcja wyciągnięcia szabli. Gracz może zdobywać doświadczenie poprzez odwiedzanie nowych wysp, otwieranie skrzyń, pokonywanie przeciwników oraz wykonywanie losowo generowanych misji pobocznych, które są wyświetlane pod minimapą. Zadania dotyczą znajdowania skrzyń oraz pokonywania przeciwników o określonych poziomach trudności w zależności od wylosowanego poziomu misji. Obrażenia zadawane i przyjmowane przez gracza są wyświetlane podczas walki, tak samo, jak i informacje o ewentualnym nietrafieniu.</p>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/RozgrywkaWalka.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 6. Widok interfejsu i walki na scenach wysp</i>
</p>

<p align="justify">Z poziomu rozgrywki gracz może otworzyć dużą mapę prezentującą rzut z góry całej sceny, menu pauzy, gdzie można zapisać lub wczytać grę bądź wyjść z gry, lub do menu głównego, oraz ekwipunek, w którym znajdują się posiadane przez gracza przedmioty. Widoczne są w nim również statystyki postaci, takie jak poziom gracza, doświadczenie, zdrowie, atak, szczęście, ilość złota, a także liczba odwiedzonych wysp, pokonanych przeciwników i wykonanych misji.</p>

<p align="justify">W menu głównym możliwe jest uruchomienie nowej gry, kontynuowanie rozgrywki od ostatniego zapisu, wczytanie dowolnego zapisu gry, wyjście z gry, a także przejście do menu opcji. Dostępne są ustawienia sterowania, grafiki, wyświetlacza, dźwięku, oraz języka, a z opcji sterowania dodatkowo można przejść do panelu edycji klawiszy. Gracz może edytować domyślnie przypisane ustawienia wedle własnych preferencji.</p>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/RozgrywkaEkwipunek.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 7. Widok ekwipunku</i>
</p><br>

<p align="center">
  <img src="https://github.com/AquilaProjectTeam/AquilaGame/blob/main/Graphics/MenuGłówne.png" style="width: 90%; height: 90%" /></p>
<p align="center">
  <i>Rys. 8. Widok przejścia z menu opcji do panelu edycji klawiszy</i>
</p>

<p align="justify">Zrealizowany projekt będący grą komputerową z pewnością wyróżnia się autorskimi obiektami, interfejsami i muzyką oraz algorytmami generowania wielu losowych elementów gry. Dzięki nim sceny generowane są za każdym razem w inny sposób, co dodaje rozgrywce różnorodności. Przygotowany został ciekawy system walki, zdobywania doświadczenia oraz wykonywania misji, a także podróżowania po oceanie statkiem między wyspami różnych typów i wielkości.</p>

---

## License Overview  

- **Visual assets** (e.g., images, textures, 3D models) are licensed under **CC BY 4.0**.  
- **All other content** (e.g., code, documentation) is proprietary and provided without a license.  
See the [LICENSE](./LICENSE) file for full details.
