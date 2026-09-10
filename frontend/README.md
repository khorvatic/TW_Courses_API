# TW Courses — frontend

Vanilla HTML/CSS/JS frontend za [TW_Courses_API](https://github.com/khorvatic/TW_Courses_API) (.NET Web API). Bez ikakvog build koraka i bez frameworka — samo `<script>` tagovi.

## Što je uključeno

- **Katalog kolegija** (javno vidljivo) s pretragom, detalj kolegija s poglavljima, ispitima i recenzijama
- **Registracija / prijava** (JWT sprema se u `localStorage`, čita se iz njega korisnički ID, email i uloge)
- **Upis na kolegij** i označavanje kao završen, pregled "Mog profila" i uređivanje računa
- **Polaganje ispita**: pokretanje pokušaja, biranje odgovora (zaključavaju se nakon odabira jer API ne dopušta studentu da ih promijeni), predaja i prikaz bodova/rezultata
- **Admin / Instructor panel**: upravljanje kolegijima, poglavljima, ispitima, pitanjima, odgovorima, korisnicima, ulogama i dodjelom uloga, te izvještaji (pokušaji ispita, recenzije) — sve s prikazom kontrola točno prema tome što API-jeve `[Authorize(Roles=...)]` politike dopuštaju danoj ulozi

## Pokretanje

### 1. API mora imati omogućen CORS

API trenutno (u `Presentation/Program.cs`) **nema CORS middleware**, pa će preglednik blokirati sve pozive s frontenda jer je na drugom originu/portu. Dodaj u `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
```

i odmah prije `app.UseAuthentication();` dodaj:

```csharp
app.UseCors("AllowFrontend");
```

Bez ovoga ćeš u konzoli preglednika vidjeti CORS greške, a frontend će prikazivati "Ne mogu se spojiti na API".

### 2. Pokreni API

```bash
dotnet run --project Presentation
```

Po defaultu (`launchSettings.json`) API sluša na `http://localhost:5019`. Zadani admin račun koji API sam kreira: `admin@mail.com` / `admin123`.

### 3. Posluži frontend

Otvaranje `index.html` klikom obično radi (nema ES modula), ali preporučeno je poslužiti ga preko lokalnog servera radi konzistentnog ponašanja:

```bash
npx serve frontend
# ili
python -m http.server 8080 --directory frontend
```

Zatim otvori prikazani URL u pregledniku.

### 4. Podesi API URL (ako treba)

Klikni ⚙ ikonu u gornjem desnom kutu ako API ne radi na `http://localhost:5019/api`. Vrijednost se pamti u `localStorage`.

## Struktura

```
frontend/
  index.html
  css/styles.css
  js/api.js          – fetch wrapper + pozivi za svaki resurs API-ja
  js/auth.js          – JWT dekodiranje, sesija, provjera uloga
  js/ui.js            – toast, escapeHtml, formatiranje TimeSpan/DateOnly, generička CRUD tablica
  js/router.js        – hash-based router (#/course/12 i sl.)
  js/views/public.js  – katalog kolegija, detalj kolegija
  js/views/account.js – login, registracija, profil
  js/views/exam.js     – tijek polaganja ispita
  js/views/admin.js    – admin/instructor panel
  js/main.js           – bootstrap, navigacija
```

## Poznata ograničenja API-ja (ne frontenda)

- `AnswerDto` (odgovor GET-a) ne vraća je li odgovor točan (`Correct`) — pa se to polje kod uređivanja odgovora ne može unaprijed popuniti. Frontend na to jasno upozorava u admin panelu.
- Student ne može promijeniti/poništiti odabrani odgovor na ispitu (nema DELETE dozvolu), pa se odgovori "zaključavaju" odmah nakon slanja.
- Pregled poglavlja, ispita i recenzija kolegija zahtijeva prijavu (API to gate-a s `[Authorize]`), dok je sam popis kolegija javan.
