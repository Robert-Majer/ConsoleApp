namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        private IEnumerable<ImportedObject> _importedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            _importedObjects = new List<ImportedObject>() { new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);
            var importedLines = new List<string>();

            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 0; i < importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();

                if (values.Length > 1)
                {
                    importedObject.Type = values[0];
                    importedObject.Name = values[1];
                    importedObject.Schema = values[2];
                    importedObject.ParentName = values[3];
                    importedObject.ParentType = values[4];
                    importedObject.DataType = values[5];
                    if (values.Length < 7)
                        importedObject.IsNullable = null;
                    else
                        importedObject.IsNullable = values[6];
                    ((List<ImportedObject>)_importedObjects).Add(importedObject);
                }
            }

            // clear and correct imported data
            foreach (var importedObject in _importedObjects)
            {
                if (importedObject.Type != null)
                    importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                if (importedObject.Name != null)
                    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                if (importedObject.Schema != null)
                    importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                if (importedObject.ParentName != null)
                    importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                if (importedObject.ParentType != null)
                    importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
            }

            // assign number of children
            for (int i = 0; i < _importedObjects.Count(); i++)
            {
                var importedObject = _importedObjects.ToArray()[i];
                foreach (var impObj in _importedObjects)
                {
                    if (impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name)
                    {
                        importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                    }
                }
            }

            foreach (var database in _importedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in _importedObjects)
                    {
                        if (table.ParentType != null && table.ParentType.ToUpper() == database.Type && table.ParentName == database.Name)
                        {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                            // print all table's columns
                            foreach (var column in _importedObjects)
                            {
                                if (column.ParentType != null && column.ParentType.ToUpper() == table.Type && column.ParentName == table.Name)
                                {
                                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadLine();
        }
    }
}