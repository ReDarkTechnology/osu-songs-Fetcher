using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace osu_Songs_Fetcher
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public static string latestFolder;
		public static string osuSongsFolder;
		public static string fetchedSongsFolder;
		public MainForm()
		{
			InitializeComponent();
			latestFolder = "";
		}
		void Button1Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = latestFolder;
			if(folderBrowserDialog1.ShowDialog() == DialogResult.OK){
				latestFolder = folderBrowserDialog1.SelectedPath;
				osuSongsFolder = folderBrowserDialog1.SelectedPath;
				textBox1.Text = osuSongsFolder;
			}
		}
		void Button2Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = latestFolder;
			if(folderBrowserDialog1.ShowDialog() == DialogResult.OK){
				latestFolder = folderBrowserDialog1.SelectedPath;
				fetchedSongsFolder = folderBrowserDialog1.SelectedPath;
				textBox2.Text = fetchedSongsFolder;
			}
		}
		void Button3Click(object sender, EventArgs e)
		{
			osuSongsFolder = textBox1.Text;
			fetchedSongsFolder = textBox2.Text;
			if(!string.IsNullOrEmpty(osuSongsFolder) && !string.IsNullOrEmpty(fetchedSongsFolder)){
				process = true;
				if(!prelisted){
					listView1.Items.Clear();
					if(!Directory.Exists(fetchedSongsFolder)) Directory.CreateDirectory(fetchedSongsFolder);
					foundFolders = Directory.GetDirectories(osuSongsFolder);
				}
				currentFolder = 0;
			}
		}
		public void Log(object obj){
			textBox3.Text = obj.ToString() + Environment.NewLine + textBox3.Text;
		}
		//Async
		public bool process;
		public bool prelisted;
		public string[] foundFolders;
		public int currentFolder;
		public bool processList;
		public List<OsuReader.OsuMap> maps = new List<OsuReader.OsuMap>();
		public List<ListViewItem> mapsListed = new List<ListViewItem>();
		public int preloadInt;
		public bool ae;
		void Timer1Tick(object sender, EventArgs e)
		{
			if(process){
				if(!prelisted){
					if(currentFolder < foundFolders.Length - 1){
						string dir = foundFolders[currentFolder];
						var foundOsuFiles = new List<string>();
						string[] foundFiles = Directory.GetFiles(dir);
						Log("Found directory : " + dir);
						foreach(string a in foundFiles){
							if(Path.GetExtension(a) == ".osu"){
								foundOsuFiles.Add(a);
								Log("osu file detected!");
							}
						}
						if(foundOsuFiles.Count > 0){
							OsuReader.OsuMap mapInfo = OsuReader.GetOsuMapInfo(foundOsuFiles[0]);
							string audioFile = Path.Combine(dir, mapInfo.AudioFile);
							string endFile = Path.Combine(fetchedSongsFolder, mapInfo.Artist + " - " + mapInfo.Title + Path.GetExtension(audioFile));
							string secondAttempt = Path.Combine(fetchedSongsFolder, RemoveInitialNumbers(GetFolderName(dir)) + Path.GetExtension(audioFile));
							string result = "Failed...";
							if(File.Exists(audioFile)){
								try {
									File.WriteAllBytes(endFile, File.ReadAllBytes(audioFile));
									result = "Success!";
								} catch (Exception ea) {
									Log("First attempt failed : " + ea.Message);
									result = "First attempt failed : " + ea.Message;
								}
								if(result != "Success!"){
									try {
										File.WriteAllBytes(secondAttempt, File.ReadAllBytes(audioFile));
										result = "Success!";
									} catch (Exception ea) {
										Log("Fetching failed : " + ea.Message);
										result = "Fetching failed : " + ea.Message;
									}
								}
							}
							mapInfo.order = currentFolder;
							ListViewItem osuFile = AddItem(mapInfo.Title,mapInfo.Artist, dir, result);
							maps.Add(mapInfo);
							mapsListed.Add(osuFile);
						}
						currentFolder++;
					}else{
						if(!ae){
							Log("Fetching Done!");
							ae = false;
						}
						process = false;
					}
				} else{
					if(currentFolder < maps.Count){
						OsuReader.OsuMap mapInfo = maps[currentFolder];
						Log("Try to fetch : " + mapInfo.Title);
						string dir = mapsListed[currentFolder].SubItems[2].Text;
						string audioFile = Path.Combine(dir, mapInfo.AudioFile);
						string endFile = Path.Combine(fetchedSongsFolder, mapInfo.Artist + " - " + mapInfo.Title + Path.GetExtension(audioFile));
						string secondAttempt = Path.Combine(fetchedSongsFolder, RemoveInitialNumbers(GetFolderName(dir)) + Path.GetExtension(audioFile));
						string result = "Failed...";
						if(File.Exists(audioFile)){
							try {
								File.WriteAllBytes(endFile, File.ReadAllBytes(audioFile));
								result = "Success!";
							} catch (Exception ea) {
								Log("First attempt failed : " + ea.Message);
								result = "First attempt failed : " + ea.Message;
							}
							if(result != "Success!"){
								try {
									File.WriteAllBytes(secondAttempt, File.ReadAllBytes(audioFile));
									result = "Success!";
								} catch (Exception ea) {
									Log("Fetching failed : " + ea.Message);
									result = "Fetching failed : " + ea.Message;
								}
							}
						}
						mapsListed[currentFolder].SubItems[3].Text = result;
						currentFolder++;
					}else{
						if(!ae){
							Log("Fetching Done!");
							ae = true;
						}
						process = false;
					}
				}
				progressBar1.Value = currentFolder;
				progressBar1.Maximum = foundFolders.Length - 1;
				label3.Text = currentFolder.ToString() + "/" + (foundFolders.Length - 1).ToString();
			}
			if(processList){
				if(currentFolder < foundFolders.Length - 1){
					string dir = foundFolders[currentFolder];
					var foundOsuFiles = new List<string>();
					string[] foundFiles = Directory.GetFiles(dir);
					Log("Found directory : " + dir);
					foreach(string a in foundFiles){
						if(Path.GetExtension(a) == ".osu"){
							foundOsuFiles.Add(a);
							Log("osu file detected!");
						}
					}
					if(foundOsuFiles.Count > 0){
						OsuReader.OsuMap mapInfo = OsuReader.GetOsuMapInfo(foundOsuFiles[0]);
						string audioFile = Path.Combine(dir, mapInfo.AudioFile);
						string endFile = Path.Combine(fetchedSongsFolder, mapInfo.Artist + " - " + mapInfo.Title + Path.GetExtension(audioFile));
						string secondAttempt = Path.Combine(fetchedSongsFolder, RemoveInitialNumbers(GetFolderName(dir)) + Path.GetExtension(audioFile));
						string result = "...";
						result = "Idle";
						mapInfo.order = currentFolder;
						ListViewItem osuFile = AddItem(mapInfo.Title,mapInfo.Artist, dir, result);
						maps.Add(mapInfo);
						mapsListed.Add(osuFile);
					}
					currentFolder++;
				}else{
					processList = false;
					prelisted = true;
					Log("Listing Done!");
				}
				progressBar1.Value = currentFolder;
				progressBar1.Maximum = foundFolders.Length - 1;
				label3.Text = currentFolder.ToString() + "/" + (foundFolders.Length - 1).ToString();
			}
		}
		public ListViewItem AddItem(string title, string artist, string folder, string status){
			var subItems = new List<ListViewItem.ListViewSubItem>();
			var subItem1 = new ListViewItem.ListViewSubItem();
			subItem1.Text = title;
			subItems.Add(subItem1);
			var subItem2 = new ListViewItem.ListViewSubItem();
			subItem2.Text = artist;
			subItems.Add(subItem2);
			var subItem3 = new ListViewItem.ListViewSubItem();
			subItem3.Text = folder;
			subItems.Add(subItem3);
			var subItem4 = new ListViewItem.ListViewSubItem();
			subItem4.Text = status;
			subItems.Add(subItem4);
			var newItem = new ListViewItem(subItems.ToArray(), 0);
			listView1.Items.Add(newItem);
			return newItem;
		}
		public string GetFolderName(string path){
			string[] paths = path.Split(Path.DirectorySeparatorChar);
			return paths[paths.Length - 1];
		}
		public string RemoveInitialNumbers(string text){
			var numbers = new List<string>(){
				"1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
			};
			var owo = text.Split((" ").ToCharArray());
			string result = text;
			try {
				if(numbers.Contains(owo[0].Substring(0,1)) && numbers.Contains(owo[0].Substring(1,1))){
					result = text.Remove(0, owo[0].Length + 1);
				}
			} catch {
				result = text;
			}
			return result;
		}
		void Button4Click(object sender, EventArgs e)
		{
			osuSongsFolder = textBox1.Text;
			fetchedSongsFolder = textBox2.Text;
			if(!string.IsNullOrEmpty(osuSongsFolder) && !string.IsNullOrEmpty(fetchedSongsFolder)){
				processList = true;
				listView1.Items.Clear();
				if(!Directory.Exists(fetchedSongsFolder)) Directory.CreateDirectory(fetchedSongsFolder);
				foundFolders = Directory.GetDirectories(osuSongsFolder);
				currentFolder = 0;
				prelisted = false;
			}
		}
	}
}
