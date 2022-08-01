
/* Komentarze
#Linia16  Tworzenie oraz dodawanie do listy ImportedObjects instacji klasy ImportedObject mija się z celem. 
#Linia27 W pętli for należy dodać warunek sprawdzający czy lista values posiada wartość na danym indeksie. 
    Również warto rozpocząć pętlę od licznika =1, jako ,że pierwszy element to nazwy kolumn tabeli. 
#Także zamiast znaku <= należy zostosować < bądź skrócić listę importedLines - 1;
#Linia43 W pętli foreach, zbedne jest stosowanie metody Replace(), jako ,iż metoda trim obejmuje usuwanie spacji oraz nowych linii. 
#Linia55 Tworzenie tablicy importedObject jest zbędne jeśli zastosować metodę ElementAt() do listy która jest iterowana.
#Linia62 Możliwe jest skrócenie wyrażeniem +=1
#Linia70 Warto rozważyć zastosowanie metody ToUpper przy wszystkich porównianiach pól. Kewstia czy zapis małymi bądź dużymi literami jest celowy czy jest wynikiem błędu. 
Klasę bazową warto zadeklarować jako klasę abstrakcyjną bądź interfejs. Rozważyc również stosowanie wlascicowsci dla wszystkich pól klasy.
Pole NumberOfChildren zawierać będzie jedynie liczby całkowite więc należy zadeklarować jako int bądź long. 
*/


namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>();

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 1; i < importedLines.Count; i++)
            {
                //var importedLine = importedLines[i];
                var values = importedLines[i].Split(';');
                var importedObject = new ImportedObject();
                importedObject.Type = (values.Length > 0) ? values[0] : String.Empty;
                importedObject.Name = (values.Length > 1) ? values[1] : String.Empty;
                importedObject.Schema = (values.Length > 2) ? values[2] : String.Empty;
                importedObject.ParentName = (values.Length > 3) ? values[3] : String.Empty;
                importedObject.ParentType = (values.Length > 4) ? values[4] : String.Empty;
                importedObject.DataType = (values.Length > 5) ? values[5] : String.Empty;
                importedObject.IsNullable = (values.Length > 6) ? values[6] : String.Empty;
                ((List<ImportedObject>)ImportedObjects).Add(importedObject);
            }

            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.Type = importedObject.Type.Trim().ToUpper();
                importedObject.Name = importedObject.Name.Trim();
                importedObject.Schema = importedObject.Schema.Trim();
                importedObject.ParentName = importedObject.ParentName.Trim();
                importedObject.ParentType = importedObject.ParentType.Trim();
            }

            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                //var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == ImportedObjects.ElementAt(i).Type)
                    {
                        if (impObj.ParentName == ImportedObjects.ElementAt(i).Name)
                        {
                            ImportedObjects.ElementAt(i).NumberOfChildren += 1; //= 1 + ImportedObjects.ElementAt(i).NumberOfChildren;
                        }
                    }
                }
            }

            //#####################################################################

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        if (table.ParentType.ToUpper() == database.Type)
                        {
                            if (table.ParentName.ToUpper() == database.Name.ToUpper())
                            {
                                Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                // print all table's columns
                                foreach (var column in ImportedObjects)
                                {
                                    if (column.ParentType.ToUpper() == table.Type)
                                    {
                                        if (column.ParentName.ToUpper() == table.Name.ToUpper())
                                        {
                                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public int NumberOfChildren { get; set; }
    }

    interface ImportedObjectBaseClass
    {
        string Name { get; set; }
        string Type { get; set; }
    }
}
