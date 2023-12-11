using INTERPRETE_C__to_HULK;

namespace G_Wall_E
{
	public class Import
	{
		const string Base_Directory = "Saved_Code/";
		const string Extension_Directory = ".txt";
		string Dir;
		
		public Import(string file_to_import)
		{
			Dir = Base_Directory + file_to_import + Extension_Directory;
		}
		
		public string Code()
		{
			System.IO.StreamReader sr = new System.IO.StreamReader(Dir);
			string code = sr.ReadToEnd();
			
			return code;
		}
	}
}
