﻿/* If you're reading this then you are looking at my disgusting source that allows duck game to host 8 players
   rather than the basic 4. 

   There are various things that need to be done in order to allow duck game to host more than 4 players without crashing outright.
*/

using System;
using System.IO;
using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("8 Player Duck Game Alpha")]

// The author of the mod
[assembly: AssemblyCompany("Ainsley Harriott (Jellybaby44)")]

// The description of the mod
[assembly: AssemblyDescription("Edits various bits of duck game to allow up to 8 players to join")]

// The mod's version
[assembly: AssemblyVersion("0.0.0.3")]

namespace DuckGame.IncreasedPlayerLimit
{

    public class Injection
    {

        private void injectionMethod1()
        {
            return;
        }

        public static void install(int typereplace, string methodtoreplace, string methodtoinject)
        {
            Type typereplace1 = null;
            Type typeinject1 = null;
            if (typereplace == 0)
            {
                typereplace1 = typeof(Duck);
                typeinject1 = typeof(Injection);
                
            }
            else if (typereplace == 1)
            {
                typereplace1 = typeof(DuckNetworkCore);
                typeinject1 = typeof(DuckNetworkCoreEdits);
            }
            else if (typereplace == 2)
            {
                typereplace1 = typeof(Input);
                typeinject1 = typeof(InputEdits);
            }
            else if (typereplace == 3)
            {
                typereplace1 = typeof(TeamSelect2);
                typeinject1 = typeof(TeamSelect2Edits);
            }
            MethodInfo methodToReplace = typereplace1.GetMethod(methodtoreplace, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo methodToInject = typeinject1.GetMethod(methodtoinject, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            UnsafeCode.CodeInjection(methodToReplace, methodToInject);

            /*
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                    Console.WriteLine("\nVersion x84 Debug\n");

                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;

                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    *tar = *inj;
#endif
                }
                else
                {

                    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                    Console.WriteLine("\nVersion x64 Debug\n");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;


                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    *tar = *inj;
#endif
                }
            }*/
        }

    }

    public class IncreasedPlayerLimit : Mod
    {
		// The mod's priority; this property controls the load order of the mod.
		public override Priority priority
		{
			get { return Priority.Highest; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Injection.install(2, "InitDefaultProfiles", "InitDefaultProfiles");

            InputEdits.Initialize();
            InputEdits.InitDefaultProfiles();

            InputProfileCoreEdits.defaultProfiles();
            PersonaEdits._personas();

            Injection.install(1, "RecreateProfiles", "RecreateProfiles");

            TeamSelect2Edits.OnlineSettings();
//            Injection.install(3, "OnNetworkConnecting", "OnNetworkConnecting"); // Crashes when game calls OnSessionEnd
                                                                                  // Will implement when it doesn't crash.


            Injection.install(0, "UpdateQuack", "injectionMethod1"); // Disables quack to check everything loaded right
                                                                     // Won't be needed in full release

            // Base
            base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
		}


        // Used to load the embedded DLL rather than have the user put it in the Duck Game folder
        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Load();
        }

        public static Assembly Load()
        {
            byte[] ba = null;
            //string resource = "DuckGame.IncreasedPlayerLimit.CodeInjectionDLL.dll";
            string resource = "IncreasedPlayerLimit.CodeInjectionDLL.dll";
            Assembly curAsm = Assembly.GetExecutingAssembly();
            using (Stream stm = curAsm.GetManifestResourceStream(resource))
            {
                ba = new byte[(int)stm.Length];
                stm.Read(ba, 0, (int)stm.Length);

                return Assembly.Load(ba);
            }
        }

    }
}
