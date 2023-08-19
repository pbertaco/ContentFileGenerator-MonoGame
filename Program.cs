namespace ContentFileGenerator
{
    internal class Program
    {
        static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".spritefont" };

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ContentMgcbGenerator.exe <input-folder>");
                return;
            }

            string inputFolder = args[0];
            if (!Directory.Exists(inputFolder))
            {
                Console.WriteLine("The specified input folder does not exist.");
                return;
            }

            if (!inputFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                inputFolder += Path.DirectorySeparatorChar;
            }

            string mgcbFilePath = Path.Combine(inputFolder, "Content.mgcb");

            using (StreamWriter mgcbFile = new StreamWriter(mgcbFilePath))
            {
                mgcbFile.WriteLine();
                mgcbFile.WriteLine("#----------------------------- Global Properties ----------------------------#");
                mgcbFile.WriteLine();
                mgcbFile.WriteLine("/outputDir:bin");
                mgcbFile.WriteLine("/intermediateDir:obj");
                mgcbFile.WriteLine("/platform:Windows");
                mgcbFile.WriteLine("/config:");
                mgcbFile.WriteLine("/profile:Reach");
                mgcbFile.WriteLine("/compress:False");
                mgcbFile.WriteLine();
                mgcbFile.WriteLine("#-------------------------------- References --------------------------------#");
                mgcbFile.WriteLine();
                mgcbFile.WriteLine();
                mgcbFile.WriteLine("#---------------------------------- Content ---------------------------------#");
                mgcbFile.WriteLine();

                ProcessDirectory(inputFolder, inputFolder, mgcbFile);

                mgcbFile.WriteLine();
            }

            Console.WriteLine("Content.mgcb file generated successfully.");
        }

        static void ProcessDirectory(string baseFolder, string currentFolder, StreamWriter mgcbFile)
        {
            string[] files = Directory.GetFiles(currentFolder, "*.*");

            foreach (string file in files)
            {
                if (file.Equals(mgcbFile.BaseStream))
                {
                    continue;
                }

                string relativePath = GetRelativePath(baseFolder, file);
                string extension = Path.GetExtension(file).ToLower();

                if (AllowedExtensions.Contains(extension))
                {
                    string contentRef = Path.ChangeExtension(relativePath, null).Replace(Path.DirectorySeparatorChar, '/');
                    contentRef = contentRef.TrimStart('/');

                    if (extension.Equals(".spritefont"))
                    {
                        mgcbFile.WriteLine("# begin " + relativePath);
                        mgcbFile.WriteLine("/importer:FontDescriptionImporter");
                        mgcbFile.WriteLine("/processor:FontDescriptionProcessor");
                        mgcbFile.WriteLine("/processorParam:PremultiplyAlpha=True");
                        mgcbFile.WriteLine("/processorParam:TextureFormat=Compressed");
                        mgcbFile.WriteLine("/build:" + relativePath);
                        mgcbFile.WriteLine();
                    }
                    else
                    {
                        mgcbFile.WriteLine("# begin " + relativePath);
                        mgcbFile.WriteLine("/importer:TextureImporter");
                        mgcbFile.WriteLine("/processor:TextureProcessor");
                        mgcbFile.WriteLine("/processorParam:ColorKeyColor=255,0,255,255");
                        mgcbFile.WriteLine("/processorParam:ColorKeyEnabled=True");
                        mgcbFile.WriteLine("/processorParam:GenerateMipmaps=False");
                        mgcbFile.WriteLine("/processorParam:PremultiplyAlpha=True");
                        mgcbFile.WriteLine("/processorParam:ResizeToPowerOfTwo=False");
                        mgcbFile.WriteLine("/processorParam:MakeSquare=False");
                        mgcbFile.WriteLine("/processorParam:TextureFormat=Color");
                        mgcbFile.WriteLine("/build:" + relativePath);
                        mgcbFile.WriteLine();
                    }
                }
            }

            string[] subDirectories = Directory.GetDirectories(currentFolder);

            foreach (string subDir in subDirectories)
            {
                ProcessDirectory(baseFolder, subDir, mgcbFile);
            }
        }

        static string GetRelativePath(string baseDirectory, string fullPath)
        {
            Uri baseUri = new Uri(baseDirectory);
            Uri fullPathUri = new Uri(fullPath);
            string relativePath = baseUri.MakeRelativeUri(fullPathUri).ToString();

            if (relativePath.Contains("%20"))
                relativePath = Uri.UnescapeDataString(relativePath);

            return relativePath;
        }
    }
}