
namespace MOLToXYZCLI
{
    /// <summary>
    /// Runs the MOLToXYZCLI Tool
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main Method of the Program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No Arguments Provided, Please provide a file path to a .mol file");
                return;
            }

            string fullFilePath = Path.GetFullPath(args[0]);

            if (File.Exists(fullFilePath))
            {
                DisplayHeader();
                ParseFile(fullFilePath);
            }
            else if (Directory.Exists(fullFilePath))
            {
                DisplayHeader();
                ParseFolder(fullFilePath);
            }
            else
            {
                Console.WriteLine("The path does not exist.");
                return;
            }
        }

        /// <summary>
        /// Displays the Tool Banner and Signature
        /// </summary>
        private static void DisplayHeader()
        {
            try
            {
                var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var assemblyPath = Path.GetDirectoryName(assemblyLocation);
                if (assemblyPath == null)
                    return;

                string BannerPath = Path.Combine(assemblyPath, "Resources", "Banner.txt");
                string SignaturePath = Path.Combine(assemblyPath, "Resources", "Signature.txt");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(File.ReadAllText(BannerPath));

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(File.ReadAllText(SignaturePath));
            }
            finally
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Parses the Specified Folder, Grabs all Files with the .mol extension and calls the ParseFile Method
        /// </summary>
        /// <param name="folderPath"> The Path to the Folder </param>
        private static void ParseFolder(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.mol");

            foreach (string file in files)
            {
                if (File.Exists(file))
                    ParseFile(file);
            }
        }

        /// <summary>
        /// Parses a File and Converts it to a .xyz file
        /// </summary>
        /// <param name="fullFilePath"></param>
        public static void ParseFile(string fullFilePath)
        {
            if (!fullFilePath.ToLower().Contains(".mol"))
            {
                Console.WriteLine("File is not a .mol file");
                return;
            }

            if (!File.Exists(fullFilePath))
            {
                Console.WriteLine("File does not exist");
                return;
            }

            CreateXYZFile(fullFilePath);
        }

        /// <summary>
        /// Gets the XYZ Header
        /// </summary>
        /// <param name="fileName"> The Name of the File and therefor the name of the Molecule </param>
        /// <param name="atomNumber"> The Number of Atoms Present in the .mol file </param>
        /// <returns> The Header for the XYZ File </returns>
        private static string GetHeader(string fileName, int atomNumber)
        {
            return $"{atomNumber}\n{fileName}\n";
        }

        /// <summary>
        /// Converts a line from the .mol file that contains the Atom Information to the XYZ format line
        /// </summary>
        /// <param name="line"> The .mol line to convert formats </param>
        /// <returns> The .xyz line to add to the File </returns>
        public static string ConvertMOLLineToXYZ(string line)
        {
            List<string> array = line.TrimStart().Split(' ').ToList();

            array.RemoveAll(item => item == "");

            return string.Join(" ", new string[] { array[3], array[0], array[1], array[2] });
        }

        /// <summary>
        /// Creates the XYZ File and Saves it in the same Directory as the .mol file, Creates the header and then loops through the remaining lines depdening on the number of atoms and formats them to the XYZ format
        /// </summary>
        /// <param name="filePath"> The Path to the File to Convert </param>
        public static void CreateXYZFile(string filePath)
        {
            string fullFilePath = Path.GetFullPath(filePath);
            string fileName = Path.GetFileName(fullFilePath);
            string[] MOLFile = File.ReadAllLines(fullFilePath);
            int atomNumber = int.Parse(MOLFile[3].Split(' ')[1]);
            string XYZFile = GetHeader(fileName, atomNumber);

            for (int i = 4; i < 4 + atomNumber; i++)
                XYZFile += ConvertMOLLineToXYZ(MOLFile[i]) + "\n";

            File.WriteAllText(fullFilePath.Replace(".mol", ".xyz"), XYZFile);
            Console.WriteLine($"File {fileName} has been converted to {fileName.Replace(".mol", ".xyz")}");
        }
    }
}
