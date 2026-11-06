# JJ Poems

Minimalistická a elegantná .NET MAUI aplikácia pre pohodlné čítanie poézie na Androide, pripravená ako štartovací projekt pre Visual Studio 2026.

## Architektúra

- `Models`: doménové typy pre básne, autora a seed data.
- `Services`: lokálny JSON repository, favorites storage cez `Preferences` a jednoduchá Shell navigácia.
- `ViewModels`: MVVM vrstva bez externých balíkov, pripravená na ďalšie rozširovanie.
- `Views`: 4 hlavné karty v Shell navigácii plus detail básne mimo spodnej navigácie.
- `Resources`: farby, štýly, SVG branding a lokálne JSON dáta.

## Obrazovky a komponenty

- Domov: úvodný hero blok, logo, CTA tlačidlá a vybrané básne.
- Básne: vyhľadávanie a čisté karty s preview.
- Detail básne: čítací režim, obľúbené a predchádzajúca/ďalšia báseň.
- Obľúbené: uložené básne z `Preferences`.
- O autorovi: biografia, inšpirácie, úspechy a citát.

## Farebná paleta

- Primary background: `#F7F1E8`
- Secondary background: `#EFE4D1`
- Primary text: `#2F2924`
- Secondary text: `#6E6257`
- Accent: `#A8825B`
- Border / divider: `#D8C8B3`
- Dark primary background: `#161311`
- Dark secondary background: `#211C19`
- Dark primary text: `#F2E9DC`
- Dark secondary text: `#B9A99A`
- Dark accent: `#D3AE84`
- Dark border: `#4A3F38`

## Typografia

- Titulky a názvy básní: serif fallback cez `serif` / `Georgia` / `Times New Roman`.
- UI a bežný text: `OpenSansRegular` a `OpenSansSemibold`.
- Odporúčaný pairing pri budúcej výmene fontov: `Cormorant Garamond` + `Inter` alebo `EB Garamond` + `Manrope`.

## Logo a ikonka

- Farebná verzia: jemné pierko kreslené jedným ťahom nad horizontálnou literárnou linkou v teplých krémovo-zlatých tónoch.
- Monochromatická verzia: rovnaký symbol v jednom tmavom uhľovom odtieni pre použitie na obaloch, faviconách a razbe.
- Ikonka aplikácie: zaoblený krémový štvorec s poetickým pierkom a krátkou linkou, aby ostala čitateľná aj v malých veľkostiach.

## Spustenie vo Visual Studio 2026

1. Otvor `JJPoems.csproj` vo Visual Studio 2026.
2. Skontroluj, že je nainštalovaný .NET MAUI workload s Android supportom.
3. Nastav cieľ `net10.0-android` a vyber Android emulátor.
4. Spusti `Restore`, potom `Run`.

Ak restore narazí na firemné NuGet feedy, vytvor lokálny `NuGet.config` s platnými zdrojmi alebo ich dočasne vypni pre tento projekt.
