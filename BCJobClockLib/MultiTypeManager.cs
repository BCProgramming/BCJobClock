using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

#if !iManagerCallback

#endif
namespace BASeCamp.Configuration 
{
    public static class StringExtensions
    {
        /// <summary>
        /// this string is the regular expression, sregex is the string to test.
        /// </summary>
        /// <param name="test">Regular Expression</param>
        /// <param name="sregex">String to match</param>
        /// <returns></returns>
        public static bool TestRegex(this string test, String sregex)
        {
            // System.Text.RegularExpressions.Regex re = new Regex(test,RegexOptions.IgnoreCase);
            return System.Text.RegularExpressions.Regex.Match(test,sregex, RegexOptions.IgnoreCase).Success;




        }
    }
    public interface iManagerCallback
    {
        void ShowMessage(String message);
        void UpdateProgress(float ProgressPercentage);

    }
    class Nullcallback : iManagerCallback
    {


        #region iManagerCallback Members

        public void ShowMessage(string message)
        {
            //don't care...
        }
        public void UpdateProgress(float ProgressPercentage)
        {


        }

        #endregion
    }
    //I've noticed something. Something bad. When BaseBlock starts up, it  takes way to damned long for it to start. Why? because the LoadedTypeManagers()
    //are currently being instantiated one by one. The bad thing is that they all end up opening the same assemblies and inspecting the same files, so, Instead of doing it that way
    //I am going to change it so that the LoadedTypeManagers are instead dependent on this class

    //these two classes really would make good material for a blog post or article or something...

    /// <summary>
    /// MultiTypeManager: used to load a group of LoadedTypeManager's simultaneously (that is, MultiTypeManager does the assembly looping and
    /// then calls into each LoadedTypeManager() to see if it is "allowed" to add it...
    /// </summary>
    public class MultiTypeManager
    {
        public Dictionary<Type, LoadedTypeManager> loadeddata;
        public delegate void TypeManagerLoadProgressCallback(float CurrentProgress);
        private Assembly[] useassemblies;


        public LoadedTypeManager this[Type index]
        {
            get
            {
                if (!loadeddata.ContainsKey(index))
                {
                    //add it...
                    throw new ApplicationException("Type " + index.FullName + " not enumerated...");


                }
                return loadeddata[index];

            }
        }


        public static Assembly[] AssembliesFromStrings(String[] foldernames)
        {
            return AssembliesFromStrings(foldernames, new Nullcallback());

        }

        public static Assembly[] AssembliesFromStrings(String[] foldernames, iManagerCallback datahook)
        {
            List<Assembly> buildlist = new List<Assembly>();
            buildlist.Add(Assembly.GetExecutingAssembly());
            foreach (String loopfolder in foldernames)
            {
                if (Directory.Exists(loopfolder))
                {
                    foreach (FileInfo loopfile in new DirectoryInfo(loopfolder).GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                    {
                        Assembly testassembly;
                        try
                        {
                            testassembly = Assembly.LoadFile(loopfile.FullName);
                            if (testassembly != null)
                                buildlist.Add(testassembly);

                        }
                        catch (Exception err)
                        {
                            //Debug.Print("failed to load assembly:" + loopfile.FullName + " " + err.Message);
                            datahook.ShowMessage("failed to load assembly:" + loopfile.FullName + " " + err.Message);

                        }

                    }







                }


            }



            return buildlist.ToArray();



        }
        public MultiTypeManager(String[] lookfolders, Type[] typesload, IEnumerable<String> IgnoreAssemblies,
            iManagerCallback pcallback, TypeManagerLoadProgressCallback pp, Assembly[] preloadedassemblies)
            : this(AssembliesFromStrings(lookfolders), typesload, IgnoreAssemblies, pcallback, pp, preloadedassemblies)
        {


        }
        public MultiTypeManager(Assembly[] checkassemblies, Type[] typesload, IEnumerable<String> IgnoreAssemblies, iManagerCallback pcallback, TypeManagerLoadProgressCallback pprog, Assembly[] preloadedassemblies)
            : this(checkassemblies, typesload, IgnoreAssemblies, new List<String>(), pcallback, pprog, preloadedassemblies)
        {

        }

        public MultiTypeManager(Assembly[] lookassemblies, Type[] typesload, IEnumerable<String> IgnoreAssemblies, IEnumerable<String> IncludeAssemblies, iManagerCallback pcallback, TypeManagerLoadProgressCallback pprog, Assembly[] preloadedassemblies)
        {
            //each Type corresponds to a new LoadedTypeManager to load for that type.
            //LoadedTypeManager[] ltm = new LoadedTypeManager[typesload.Length];

            //create the Dictionary first...
            if (IgnoreAssemblies == null) IgnoreAssemblies = new String[] { };
            List<Assembly> buildcheck = new List<Assembly>();
            if (preloadedassemblies != null)
                buildcheck.AddRange(preloadedassemblies);
            buildcheck.AddRange(lookassemblies);

            Assembly[] checkassemblies = buildcheck.ToArray();

            loadeddata = new Dictionary<Type, LoadedTypeManager>();
            foreach (Type looptype in typesload)
            {
                LoadedTypeManager addtm = new LoadedTypeManager(looptype, pcallback);
                loadeddata.Add(looptype, addtm);

            }



            //now that we have the LoadedTypeManager objects, uninitialized- we can iterate through each assembly and the types
            //in each assembly for matches.
            //assemblyprogressincrement: the amount of progress we will go through after checking each assembly.
            float assemblyprogressincrement = 1f / (float)checkassemblies.Count();
            float currprogress = 0;

            foreach (Assembly loopassembly in checkassemblies)
            {
                if ((loopassembly.GetName().Name.StartsWith("script_", StringComparison.OrdinalIgnoreCase)) || (!IgnoreAssemblies.Any((w) => loopassembly.GetName().Name.TestRegex(w))))
                {
                    pcallback.ShowMessage("Inspecting Assembly:" + loopassembly.GetName().Name);
                    //iterate through each type...
                    currprogress += assemblyprogressincrement;
                    Type[] typesiterate;
                    try
                    {
                        typesiterate = loopassembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException rtle)
                    {
                        pcallback.ShowMessage("ReflectionTypeLoadException occured;" + rtle.StackTrace +
                                              " InnerExceptions:");
                        foreach (Exception loopexception in rtle.LoaderExceptions)
                        {
                            pcallback.ShowMessage("RTLE Loader Exception:" + loopexception.Message + " Source:" +
                                                  loopexception.Source + "stack Trace:" + loopexception.StackTrace);



                        }
                        pcallback.ShowMessage("End of RTLE Loader Exceptions");
                        currprogress += assemblyprogressincrement;
                        if (pprog!=null) pprog(currprogress);
                        typesiterate = null;
                    }
                    if (typesiterate != null)
                    {
                        //iterate through each type...

                        //get appropriate percent per type...
                        // float percentpertype = assemblyprogressincrement / typesiterate.Length;

                        foreach (Type looptype in typesiterate)
                        {
                            //currprogress+=percentpertype;

                         

                            // pcallback.ShowMessage("Checking type:" + looptype.FullName);
                            //And... for each type, iterate through all the LoadedTypeManagers in our dictionary...
                            foreach (var checkmanager in loadeddata)
                            {
                                if (LoadedTypeManager.CheckType(looptype, checkmanager.Key, pcallback))
                                {
                                    pcallback.ShowMessage("Type:" + looptype.FullName + " is a, or implements, " +
                                                          checkmanager.Key.Name);
                                    //add it to that manager...
                                    checkmanager.Value.ManagedTypes.Add(looptype);
                                }


                            }


                        }
                    }
                }
                else
                {
                    currprogress += assemblyprogressincrement;
                    pprog(currprogress);
                    pcallback.ShowMessage("Skipped Assembly " + loopassembly.FullName);
                }
            }

            //at the conclusion of the loop, show a summary.
            if (!(pcallback is Nullcallback))
            {
                pcallback.ShowMessage("Assembly enumeration complete.(" + checkassemblies.Count().ToString() + " Assemblies ");
                //^save time by not doing this for a Nullcallback...
                foreach (var loopltm in loadeddata)
                {
                    pcallback.ShowMessage(" found " + loopltm.Value.ManagedTypes.Count + " instances of type " + loopltm.Key.Name);


                }


            }












        }

    }
    
    public class LoadedTypeManager
    {
        iManagerCallback mcallback = new Nullcallback();
        private List<Type> _ManagedTypes = new List<Type>();
        //private Dictionary<Type, IHighScoreList> directScores = new Dictionary<Type, IHighScoreList>();
        //private Dictionary<String, IHighScoreList> ScoreData = new Dictionary<String, IHighScoreList>();
        /// <summary>
        /// the Type that we are looking in assemblies for.
        /// </summary>
        Type TypeManage;
        public List<Type> ManagedTypes
        {
            get { return _ManagedTypes; }
            internal set { _ManagedTypes = value; }
        }
        public LoadedTypeManager(String lookfolder, Type lookfor, iManagerCallback pcallback)
            : this(new string[] { lookfolder }, lookfor, pcallback)
        {



        }
        /// <summary>
        /// Internal constructor used to create a LoadedTypeManager that has no actual state. This is used by the MultiTypeManager class
        /// to create "empty" TypeManager instances that it can then add to.
        /// </summary>
        /// <param name="ptypemanage"></param>
        /// <param name="pcallback"></param>
        internal LoadedTypeManager(Type ptypemanage, iManagerCallback pcallback)
        {
            TypeManage = ptypemanage;
            mcallback = pcallback;
        }

        internal LoadedTypeManager(Type ptypemanage)
            : this(ptypemanage, new Nullcallback())
        {

        }

        /*
            public Dictionary<Type, IHighScoreList> GetScoreDictionary()
            {
                return directScores;




            }
        */
        /*
        private void LoadTypeScores(Type[] typedata,iManagerCallback mcallback)
        {
            String appfolder=  BCBlockGameState.AppDataFolder;
            String hiscorefile = Path.Combine(appfolder,"Hiscores.dat");
            //open the file.
            
            if(File.Exists(hiscorefile))
            {
                mcallback.ShowMessage("Attempting to load scores from file, " + hiscorefile);
                //TODO: handle IOException error when file can't be opened for some reason.
                FileStream fread = new FileStream(hiscorefile, FileMode.Open);
                BinaryFormatter xformatter = new BinaryFormatter();
                ScoreData = (Dictionary<String, IHighScoreList>) xformatter.Deserialize(fread);
                fread.Close();
                //create our "typed" Dictionary.
                mcallback.ShowMessage("loaded " + directScores.Count + " Lists.");
                directScores = new Dictionary<Type, IHighScoreList>();
                foreach (String keyloop in ScoreData.Keys)
                {
                    Type madetype = Type.GetType(keyloop);
                    if (madetype != null)
                    {
                        directScores.Add(madetype, ScoreData[keyloop]);


                    }




                }


            }
            else
            {
                mcallback.ShowMessage("Hiscore file, " + hiscorefile + " not found. Creating empty score list.");
                //create a new one.
                ScoreData = new Dictionary<string, IHighScoreList>();
                directScores = new Dictionary<Type, IHighScoreList>();
                foreach (Type looptype in typedata)
                {
                    if(looptype.GetInterfaces().Contains(typeof(iLevelSetBuilder)))
                    {
                        mcallback.ShowMessage("Creating scorelist for type, " + looptype.FullName);
                        IHighScoreList createdscore = new LocalHighScores(20);
                        ScoreData.Add(looptype.FullName, createdscore);
                        directScores.Add(looptype, createdscore);
                    }

                }




            }


        }
        */
        ~LoadedTypeManager()
        {
            //save the highscores set
            /*
            Debug.Print("LoadedTypeManager destructor saving highscores....");
            String appfolder = BCBlockGameState.AppDataFolder;
            String Hiscorefile = Path.Combine(appfolder,"Hiscores.dat");
            Debug.Print("Saving to " + Hiscorefile);
  
                //Open the file...
                FileStream fwrite = new FileStream(Hiscorefile, FileMode.Create);
                BinaryFormatter xformatter = new BinaryFormatter();
                xformatter.Serialize(fwrite, ScoreData);
                //close
                fwrite.Close();
                fwrite.Dispose();
            */




        }
        public LoadedTypeManager(Assembly[] lookassemblies, Type lookfor, iManagerCallback pcallback)
            : this(lookassemblies, lookfor, null, pcallback)
        {


        }
        internal LoadedTypeManager(Type pTypeManage, Type[] pmanagedtypes)
            : this(pTypeManage, pmanagedtypes, new Nullcallback())
        {

        }
        /// <summary>
        /// Directly initializes a LoadedTypeManager instance with a specified list of managed types and the Type being managed.
        /// </summary>
        /// <param name="pTypeManage"></param>
        /// <param name="pmanagedtypes"></param>
        /// <param name="pcallback"></param>
        internal LoadedTypeManager(Type pTypeManage, Type[] pmanagedtypes, iManagerCallback pcallback)
        {
            TypeManage = pTypeManage;
            ManagedTypes = pmanagedtypes.ToList();
            mcallback = pcallback;
        }

        public LoadedTypeManager(Assembly[] lookassemblies, Type lookfor, IEnumerable<string> ignoreassemblynames, iManagerCallback pcallback)
        {
            TypeManage = lookfor;
            LoadTypes(lookassemblies, ignoreassemblynames);
            //LoadTypeScores(ManagedTypes.ToArray(), mcallback);


        }
        public LoadedTypeManager(string[] slookfolders, Type lookfor, iManagerCallback pcallback)
            : this(slookfolders, lookfor, new String[] { }, pcallback)
        {


        }

        public LoadedTypeManager(string[] slookfolders, Type lookfor, IEnumerable<string> ignoreassemblynames, iManagerCallback pcallback)
        {
            //alright, iterate through all the folders..
            mcallback = pcallback;
            TypeManage = lookfor;









            DirectoryInfo[] lookfolders = new DirectoryInfo[slookfolders.Length];
            for (int i = 0; i < slookfolders.Length; i++)
            {
                lookfolders[i] = new DirectoryInfo(slookfolders[i]);




            }

            List<Assembly> AssemblyList = new List<Assembly>();

            foreach (DirectoryInfo loopfolder in lookfolders)
            {
                //now iterate through all the DLL files in that folder.
                if (loopfolder.Exists)
                {
                    foreach (FileInfo dllfile in loopfolder.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                    {
                        Assembly LoadAssembly = null;
                        //attempt to load this as an assembly...
                        try
                        {
                            LoadAssembly = Assembly.LoadFile(dllfile.FullName);
                            mcallback.ShowMessage("Loaded Assembly:" + dllfile.Name);


                        }
                        catch (Exception error)
                        {
                            mcallback.ShowMessage("Load of " + dllfile.Name + " failed- " + error.Message);



                        }

                        if (LoadAssembly != null)
                        {
                            if (!ignoreassemblynames.Any<String>((y) => y.Equals(LoadAssembly.GetName().Name, StringComparison.OrdinalIgnoreCase)))
                                AssemblyList.Add(LoadAssembly);





                        }




                    }
                }

            }
            mcallback.ShowMessage("Found " + AssemblyList.Count.ToString() + " Assemblies.");
            AssemblyList.Add(Assembly.GetExecutingAssembly()); //add this assembly, so it can find the default builder.
            LoadTypes(AssemblyList.ToArray());
            //LoadTypeScores(ManagedTypes.ToArray(), mcallback);




        }
        private List<T> RemoveDuplicates<T>(List<T> removefrom)
        {
            return RemoveDuplicates(removefrom, new Nullcallback());


        }

        private List<T> RemoveDuplicates<T>(List<T> removefrom, iManagerCallback mcallback)
        {
            Dictionary<T, T> Dictcheck = new Dictionary<T, T>();


            foreach (T item in removefrom)
            {
                if (!Dictcheck.ContainsKey(item))
                {
                    Dictcheck.Add(item, item);
                }
                else
                {
                    mcallback.ShowMessage("removing duplicate entry for " + item.ToString());
                }




            }


            return Dictcheck.Keys.ToList();

        }

        private void LoadTypes(Assembly[] useassemblies)
        {
            LoadTypes(useassemblies, null);

        }

        private void LoadTypes(Assembly[] useassemblies, IEnumerable<String> ignoreassemblies)
        {
            //strip duplicates first.
            //Notice: this code is mostly dead! consult MultiTypeManager...
            if (ignoreassemblies == null) ignoreassemblies = new String[] { "" };
            useassemblies = RemoveDuplicates<Assembly>(useassemblies.ToList()).ToArray();
            foreach (var LoadAssembly in useassemblies)
            {

                int CountInFile = 0;
                Assembly assembly = LoadAssembly; //prevent access to local closure
                if (assembly.GetName().Name.Equals("script_testscript", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Print("test");
                }
                if ((!assembly.GetName().Name.StartsWith("script_", StringComparison.OrdinalIgnoreCase)) && ignoreassemblies.Any((y) => y.TestRegex(assembly.GetName().Name)))
                {
                    mcallback.ShowMessage("Skipping Assembly:" + LoadAssembly.GetName().Name);
                }
                else
                {


                    mcallback.ShowMessage("Inspecting Assembly:" + LoadAssembly.GetName().Name);
                    try
                    {

                        foreach (Type looptype in LoadAssembly.GetTypes())
                        {

                            CountInFile += CheckType(looptype) ? 1 : 0;
                        }
                    }
                    catch (ReflectionTypeLoadException rtle)
                    {
                        mcallback.ShowMessage("ReflectionTypeLoadException occured;" + rtle.StackTrace +
                                              " InnerExceptions:");
                        foreach (Exception loopexception in rtle.LoaderExceptions)
                        {
                            mcallback.ShowMessage("RTLE Loader Exception:" + loopexception.Message + " Source:" +
                                                  loopexception.Source + "stack Trace:" + loopexception.StackTrace);



                        }
                        mcallback.ShowMessage("End of RTLE Loader Exceptions");


                    }
                    catch (Exception ex)
                    {
                        mcallback.ShowMessage(ex.Message + " stack:" + ex.StackTrace);


                    }
                }
                mcallback.ShowMessage("Found " + CountInFile.ToString() + " " + TypeManage.Name + " Implementations in " + LoadAssembly.GetName());
                foreach (Type looptype in ManagedTypes)
                {
                    mcallback.ShowMessage(TypeManage.Name + " Implemented by:" + looptype.Name);



                }
            }
            mcallback.ShowMessage("Assembly enumeration complete. Removing duplicates...");
            _ManagedTypes = RemoveDuplicates(_ManagedTypes, mcallback);


        }
        public bool CheckType(Type looptype)
        {
            if (CheckType(looptype, TypeManage, mcallback))
            {
                ManagedTypes.Add(looptype);
                return true;
            }

            return false;
        }

        public static bool CheckType(Type looptype, Type TypeManage, iManagerCallback mcallback)
        {
            int CountInFile = 0;
            //if (BCBlockGameState.Verbose) mcallback.ShowMessage("checking:" + looptype.Name);
            if (!looptype.IsAbstract)
            {

                //added more recently: check wether it is a class derived from the given type.
                if (looptype.IsSubclassOf(TypeManage))
                {
                    mcallback.ShowMessage("Found " + TypeManage.Name + " Derivation:" +
                                          looptype.Name);

                    //ManagedTypes.Add(looptype);
                    CountInFile++;
                    //break out of the immediate loop.

                }
                else
                {


                    foreach (var loopinterface in looptype.GetInterfaces())
                    {
                        try
                        {
                            if (loopinterface.Equals(TypeManage))
                            {
                                mcallback.ShowMessage("Found " + TypeManage.Name + " Implementor:" +
                                                      looptype.Name);

                                //ManagedTypes.Add(looptype);
                                CountInFile++;
                                //break out of the immediate loop.
                                break;


                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Print("Exception:" + ex.Message);

                        }



                    }
                }
            }
            else
            {
                Debug.Print("Skipped Abstract class:" + looptype.FullName);
            }
            return CountInFile > 0;
        }
    }
}
