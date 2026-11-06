# JJ Poems 0.1.0 - prva testovacia verzia

Tato verzia je urcena na osobne testovanie v Android telefone. Profily a texty
su lokalne. E-mail nie je overene prihlasenie a neobnovi data na inom telefone.

Overene pre tento balicek: Release publish uspesny, 9 kontrol uloziska uspesnych,
podpis APK overeny cez apksigner (v1/v2/v3), verzia 0.1.0, ARM64 a x64.
Spustenie na telefone alebo emulatore a Android ponuka exportu/importu
zatial nie su overene. Pri zostaveni nebolo pripojene zariadenie ani dostupne AVD.

SHA256 APK: `14B5DE42D487A5462F9745E5DDFB944D77ED68685C536C9985E41300EB85D9D1`

## Instalacia APK

1. V priecinku `artifacts` najdi `JJPoems-0.1.0-test.apk`.
2. Prenes APK do telefonu cez USB alebo vlastne ulozisko.
3. V telefone otvor APK zo suboroveho spravcu. Ak Android vyziada povolenie
   instalacie z tohto zdroja, povol ho pre pouzity suborovy spravca.
4. Nainstaluj a otvor JJ Poems. Vytvor lokalny profil a zacni testovat.

Balicek obsahuje arm64 pre bezne aktualne telefony a x64 pre emulator.
Starsie telefony iba s 32-bit ARM procesorom tento balicek nepodporuje.
APK je podpisane lokalnym vyvojarskym klucom, nie produkcnym Google Play klucom.
Aktualizacie tejto testovacej vetvy musia pouzivat rovnaky podpisovy kluc.
Ak instalacia hlasi konflikt s existujucou instalaciou, najprv z nej exportuj
zalohu. Odinstalovanie aplikacie odstrani jej lokalne data.

## Instalacia cez USB z pocitaca

Zapni vyvojarske moznosti a USB debugging na telefone, pripoj ho a na jeho
displeji povol pristup tomuto pocitacu. V PowerShelli z korena projektu:

```powershell
& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' devices
& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' install -r '.\artifacts\JJPoems-0.1.0-test.apk'
```

## Co vyskusat

- Vytvor profil, uloz text s diakritikou a viacerymi prazdnymi riadkami.
- Rozpis novy text bez ulozenia, pockaj na hlasenie ulozeneho konceptu,
  vrat sa spat a znovu otvor novy blok. Koncept sa ma obnovit.
- Rozpis upravu existujuceho textu a skontroluj obnovu pri dalsom otvoreni.
- Skus zatvorenie aplikacie a znova otvor text aj rozpisany koncept.
- Vytvor druhy profil a over, ze texty prveho sa v nom nezobrazuju.
- Zmen temu, otvor detaily a kalendar. Skontroluj kontrast a klavesnicu.
- V profile exportuj zalohu a uloz ju mimo aplikacie. JSON obsahuje texty
  a profil v citatelnej podobe; nezdielaj ho nechtiac verejne.
- Importuj zalohu do druheho profilu. Obnova prida chybajuce texty, zachova
  datumy a neprepise existujuce texty ani nastavenia aktualneho profilu.
- Importuj rovnaku zalohu dvakrat: pri druhom importe ma pribudnut nula textov.
- Skus zrusenie vymazania aj zrusenie importu.

Export obsahuje ulozene texty a profil. Rozpisane koncepty a oblubene basne
z inspiracnej kolekcie nie su sucastou exportu. Pred vymenou telefonu koncept
najprv uloz ako text. Pri importe sa obnova profilu a jeho temy nevykonava.

## Overenie na pocitaci

`Tests/StorageChecks` pouziva skutocny kod JSON uloziska a workspace sluzby,
s nahradami Android platformy a datoveho typu textu pre beh na pocitaci.
Kontroluje obnovu poskodeneho suboru, zachovanie zalohy, subezne ukladanie,
opakovany import, zachovanie existujucich textov a datumov a odmietnutie
poskodeneho JSON. Nenahradza test Android UI ani vypadok napajania.

```powershell
dotnet run --project Tests\StorageChecks\StorageChecks.csproj
```

## Zostavenie balicka

Z priecinka `JJPoems`:

```powershell
dotnet publish JJPoems.csproj -c Release -f net10.0-android -p:AndroidPackageFormats=apk -p:EmbedAssembliesIntoApk=true
```

Podpisane APK je v `JJPoems/bin/Release/net10.0-android/publish`.
Tyzdenny ciel/streaky a cloudova synchronizacia zatial nie su implementovane.
Kalendar zobrazuje posledny datum upravy textov, nie uplnu historiu pisania.
