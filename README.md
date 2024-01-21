# Pux Cvičný úkol
Cílem úkolu je implementovat "file versioning tool" ketrý vrátí seznam přidaných, modifikovaných a odstraněných souborů ve složce. V zadání bylo, že mám řešení implementovat v **ASP.NET WebForms** avšak WebForms nepodporují novejší verze .NET a ASP.NET takže jsem se rozhodl pro implementaci backendu v .NET8 spolu s AspNetCore 8 a frontend v React.

**Jak Spustit:**
1) Naklonujte si tento repozitář
2) V terminu otevřete složku `PuxTask.Server` a zavolejte `npm install`
3) Běžte do složky`PuxTask.Server` a zavolejte `dotnet run`
    - automatickyty se spustí server i client část 
      - swagger běží na adrese`http://localhost:5085/swagger/index.html`
      - client na `http://localhost:5173/`
5) Do input fieldu zadejte absolutní cestu ke složce a stiskněte tlačítko Analyze

**Popis řešení Backend:**
 - **Ukládání dat**
   - veškerá logika spojená s ukládáním je implementována v `FileStateStorage` který implementuje interface `IFileStateStorage` což odstiňuje konkretní implemetaci díky čemuž můžeme jednoduše nahradit databázi  
   - použití databáze bylo explicitně zakázáno takže jsem se rozhodl potřebná data ukládat do `ConcurentDictinary` v Singleton service
   - nevýhodou tohoto řešení je že není perzistentní a po restartu aplikace o data přijdeme
  
 - **Business logika**
   - všechen kód spojený se samotnou logikou je ve složce `Application`
   - komunikace mezi kontrolerem a business logikou je provedena pomoci knihovny `MediatR` což je pro tento úkol naprostý overkill, ale je to vhodné pro větší projekty jelikož usnaďnuje decoupeling a zároveň umožňuje kotrolovat request pipeline každého requerstu což jsem pro ukázku využil implementací validace vstupních dat pomocí přidání validítoru do MediatR request Pipeline Behaviour. Díky tomu jsem byl schopný oddělit validační logiku od business logiky do `FolderAnalyzerValidator`
   - při implementaci jsem se snažil optimalizovat počet requestů do úložiště, jelikož pokud by se in-memory uložiště nahradilo nějakou databází nebylo by vhodné pro každý trackovaný soubor dělat jeden a více requestů do databáze
   - při implementaci jsem se snažil myslet na různé edge cases jako je např:
     - použití aplikace jak na různých systémech jelikož file paths fungují rozdílně na windows a linux
     - podpora aplikace napříč časovými zónami (datum poslední změny souboru je uloženo v UTC formátu)

- **Endpoint**
    -pokud nastane chyba způsobená uživatelem (prázdná cesta, nebo neplatná cesta) tak se vrátí status code 400 s popisem problému      

**Popis řešení Frontend:**
 - Frontend je napsaný pomocí React, pro stylování používám "LESS CSS"
 - Pokud uzivatel nevyplní žádnou cestu tak dostane error message
   - ![image](https://github.com/horczech/PuxTest/assets/5252663/70148455-d3d7-46dc-87f1-87e5fa3503fd)
 - pokud se stane nějaká chyba na serveru tak je o tom uživatel taky informován
 - integrace mezi React a ASP.NET je implementovana za pomocí Vite

# Ukázka user interface
![image](https://github.com/horczech/PuxTest/assets/5252663/b1e73517-bb22-4413-a1d7-4a5329021b54)
