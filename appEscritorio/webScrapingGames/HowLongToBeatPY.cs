using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;

namespace webScrapingGames
{
    class HowLongToBeatPy
    {
        List<string> games;

        public HowLongToBeatPy(List<string> games)
        {
            this.games = games;
        }
        void executeCommand(String commandR)
        {
            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + commandR,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                   proc.StandardOutput.ReadLine(); //resultado, no muestra nada, porque en el programa no hay ningun print
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Función para ejecutar un archivo .py en un ambiente de python instalado
        public bool RunPythonScript(string pythonScriptFilePath)
        {
            try
            { 
                string arg1 = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\PythonScripts\GamesInfo\games.txt";
                string arg2 = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\PythonScripts\GamesInfo\games_response.txt";
                executeCommand("python " + pythonScriptFilePath + " " + arg1 + " " + arg2);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Script failed: ", ex);
            }
        }
        private void CreateGamesFile(List<string> gameList)
        {
            string fileName = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\PythonScripts\GamesInfo\games.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (StreamWriter writer = File.CreateText(fileName))
            {
                foreach (string gameName in gameList)
                {
                    writer.WriteLine(gameName);
                }
                writer.Close();
            }
        }

        // Función que retorna el resultado final obtenido cargado a memoria => lista de tuplas (Nombre del juego, Tiempo)
        public List<Tuple<string, string>> ReadPythonResponse()
        {
            try
            {
                List<Tuple<string, string>> resulSet = new List<Tuple<string, string>>();
                string location = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\PythonScripts\GamesInfo\games_response.txt";
                var results = File.ReadAllLines(location);
                foreach (string result in results)
                {
                    // Obtiene el string antes de la aparición de ":"
                    string gameName = result.Substring(0, result.IndexOf(";"));
                    // Obtiene el string despues de la aparición de ":"
                    string timeToBeat = result.Substring(result.IndexOf(";") + 1);
                    resulSet.Add(Tuple.Create(gameName, timeToBeat));
                }
                return resulSet;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        // Función para obtener el tiempo de duración de un juego específico
        public List<Tuple<string, string>> GetTimeToBeat()
        {
            // Crea el archivo de juegos que será leido en python
            CreateGamesFile(games);
            // Se debe proveer la dirección del archivo .py a ejecutar (debe estar en la carpeta principal del proyecto)
            string pythonScriptPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\PythonScripts\howlongtobeat.py";
            // Ejecuta el script de python
            if (RunPythonScript(pythonScriptPath))
            {
                return ReadPythonResponse();
            }
            return null;
        }
    }
}
