using System;
using System.Collections.Generic;
using System.IO;

namespace StarCitizenUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Variables

            string input = "";
            string folderPath = "";
            string mappingsPath = "";
            string profilePath = "";
            string tempPath = Path.GetTempPath();
            List<string> controlsFiles = new List<string>();
            Dictionary<string, List<string>> profiles = new Dictionary<string, List<string>>();

            #endregion

            try
            {
                //Try to get the install folder if not default to ... default.... install folder
                if (Directory.GetCurrentDirectory() != null)
                {
                    folderPath = Directory.GetParent(Directory.GetCurrentDirectory()).ToString() + @"\StarCitizen";
                }

                #region LIVE or PTU

                if (Directory.Exists(folderPath + @"\PTU"))
                {
                    //If the PTU exists ask
                    Console.WriteLine("Live or PTU?");
                    input = Console.ReadLine();

                    if (input.ToLower() != "live" && input.ToLower() != "ptu")
                    {
                        int i = 0;
                        do
                        {
                            Console.WriteLine("type in Live or PTU...");
                            i++;
                            if (i > 3)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("WE'RE DOING IT LIVE!");
                                folderPath = folderPath + @"\LIVE\User";
                                mappingsPath = folderPath + @"\Controls\Mappings";
                                profilePath = folderPath + @"\Profiles";
                                break;
                            }
                            input = Console.ReadLine();
                        } while (input.ToLower() != "live" && input.ToLower() != "ptu");
                    }
                    else
                    {
                        folderPath = @"C:\Program Files\Roberts Space Industries\StarCitizen\" + input + @"\User";
                        mappingsPath = folderPath + @"\Controls\Mappings";
                        profilePath = folderPath + @"\Profiles";
                    }
                }
                else
                {
                    folderPath = folderPath + @"\LIVE\User";
                    mappingsPath = folderPath + @"\Controls\Mappings";
                    profilePath = folderPath + @"\Profiles";
                }

                Console.WriteLine();

                #endregion

                #region Copy to temp
                //Copy controls out
                var mappings = Directory.GetFiles(mappingsPath);

                foreach (var mapping in mappings)
                {
                    string fileName = @"\" + mapping.Substring(mappingsPath.Length + 1);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(string.Format("Copying from {0} to {1}", mapping, (tempPath + fileName)));
                    File.Copy(mapping, tempPath + fileName, true);
                    controlsFiles.Add(tempPath + fileName);
                }

                Console.WriteLine();

                //Copy profiles out
                var profilesFolder = Directory.GetDirectories(profilePath);
                foreach (var profileFolder in profilesFolder)
                {
                    profiles.Add(profileFolder, new List<string>());
                    var profileData = Directory.GetFiles(profileFolder);
                    var profileName = profileFolder.Substring(profilePath.Length + 1);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("Creating temp directory at {0}", tempPath + profileName));
                    Directory.CreateDirectory(tempPath + profileName);

                    foreach (var file in profileData)
                    {
                        string fileName = @"\" + file.Substring(profileFolder.Length + 1);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(string.Format("Copying from {0} to {1}", file, (tempPath + profileName + fileName)));
                        File.Copy(file, tempPath + profileName + fileName, true);
                        profiles[profileFolder].Add(tempPath + profileName + fileName);
                    }
                }

                Console.WriteLine();

                #endregion

                #region Delete
                //Delete directories
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Deleting {0}", folderPath));
                Directory.Delete(folderPath, true);
                #endregion

                #region Create directories
                //Recreate directories
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Format("Creating {0}", mappingsPath));
                Console.WriteLine();
                Directory.CreateDirectory(mappingsPath);
                Console.WriteLine(string.Format("Creating {0}", profilePath));
                Console.WriteLine();
                Directory.CreateDirectory(profilePath);
                foreach (var profile in profiles)
                {
                    Console.WriteLine(string.Format("Creating {0}", profile.Key));
                    Console.WriteLine();
                    Directory.CreateDirectory(profile.Key);
                }

                #endregion

                #region Copy files back

                //Copy back in controls
                foreach (var file in controlsFiles)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(string.Format("Copying from {0} to {1}", file, mappingsPath + @"\" + file.Substring(tempPath.Length + 1)));
                    File.Copy(file, mappingsPath + @"\" + file.Substring(tempPath.Length + 1), true);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("Deleting " + file));
                    File.Delete(file);
                }

                //Copy back profile shit
                foreach (var profile in profiles)
                {
                    var profileName = profile.Key.Substring(profilePath.Length + 1);

                    foreach (var file in profile.Value)
                    {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(string.Format("Copying from {0} to {1}", file, profile.Key + file.Substring(tempPath.Length + profileName.Length)));
                            File.Copy(file, profile.Key + file.Substring(tempPath.Length + profileName.Length), true);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(string.Format("Deleting " + file));
                            File.Delete(file);
                    }
                }
                #endregion

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done!");
                Console.WriteLine("Press any key to exit");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception e)
            {
                ThrowException(e.Message);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }

        private static void ThrowException(string message)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Utitlity has failed. Please review the following error message.");
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}