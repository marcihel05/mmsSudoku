# Sudoku – dorada
### Helena Marciuš
Original: https://github.com/ankrmpo/Sudoku
Moja dorada: https://github.com/marcihel05/mmsSudoku
Napravljene izmjene:
- Implementirana je jedna funkcija koja generira sudoku u ovisnosti o dimenziji, ali algoritam ne funkcionira za sudoku dimenzija 25x25, tj. generiranje predugo traje
- umjesto polja stringova/intova različitih dimenzija, za spremanje matrica koristi se lista listi stringova (List<List<string>>) kojoj se mjenja dimenzija ovisno o odabranoj igri
- kad se klikne na ćeliju, obojaju se red i stupac u kojem se nalazi ćelija (ali kad se klikne na drugu ćeliju, stupac ostaje obojan, red ne – to bi trebalo popraviti)
- ako se u polje unese krivi znak, javlja se poruka, tj. pop-up prozor
- dodana je opcija Hint koja upisuje znak u nasumično odabrano prazno polje
- dodan je zvuk na kraju igre
- omogućeno je brisanje unesenih znakova pomoći backspace-a
-pozadinska glazba i mogućnost uključivanja i isključivanja glazbe (glazba s https://www.bensound.com/ )

Moguća poboljšanja:
- Popraviti  bojanje reda i stupca odabrane ćelije
- Implementirati algoritam koji će generirati sudoku dimenzija 25x25
- Napisati pravila igre u npr. popup prozoru


