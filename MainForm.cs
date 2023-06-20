using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

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
		// Browse osu! maps button
		void Button1Click(object sender, EventArgs e)
		{
			var dialog = new CommonOpenFileDialog("Select osu! Songs folder...");
			dialog.RestoreDirectory = true;
			dialog.IsFolderPicker = true;
			if(dialog.ShowDialog() == CommonFileDialogResult.Ok){
				osuSongsFolder = dialog.FileName;
				textBox1.Text = osuSongsFolder;
				if(string.IsNullOrWhiteSpace(textBox2.Text)){
					// Auto set result folder
					fetchedSongsFolder = Path.Combine(Directory.GetParent(dialog.FileName).FullName, "Fetched");
					textBox2.Text = fetchedSongsFolder;
				}
			}
		}
		// Browse result directory button
		void Button2Click(object sender, EventArgs e)
		{
			var dialog = new CommonOpenFileDialog("Select result folder...");
			dialog.RestoreDirectory = true;
			dialog.IsFolderPicker = true;
			if(dialog.ShowDialog() == CommonFileDialogResult.Ok){
				fetchedSongsFolder = dialog.FileName;
				textBox2.Text = fetchedSongsFolder;
			}
		}
		// Fetch songs button
		void Button3Click(object sender, EventArgs e)
		{
			osuSongsFolder = textBox1.Text;
			fetchedSongsFolder = textBox2.Text;
			if(!string.IsNullOrEmpty(osuSongsFolder)){
				if(!string.IsNullOrEmpty(fetchedSongsFolder)){
					process = true;
					if(!prelisted){
						listView1.Items.Clear();
						if(!Directory.Exists(fetchedSongsFolder)) Directory.CreateDirectory(fetchedSongsFolder);
						foundFolders = Directory.GetDirectories(osuSongsFolder);
					}
					currentFolder = 0;
				}else{
					MessageBox.Show("Fetched folder input are empty. Please input a directory path in the fetched folder input", Application.ProductName);
				}
			}else{
				if(!string.IsNullOrEmpty(fetchedSongsFolder)){
					MessageBox.Show("Songs folder input are empty. Please input a directory path in the Songs folder input", Application.ProductName);
				}else{
					MessageBox.Show("Songs folder input are empty and the fetched folder input are also empty. Please input a directory path in both of the input", Application.ProductName);
				}
			}
		}
		// Log things
		public void Log(object obj){
			textBox3.Text = obj.ToString() + Environment.NewLine + textBox3.Text;
		}
		// Processing
		public bool process;
		public bool prelisted;
		public string[] foundFolders;
		public int currentFolder;
		public bool processList;
		public List<OsuReader.OsuMap> maps = new List<OsuReader.OsuMap>();
		public List<ListViewItem> mapsListed = new List<ListViewItem>();
		public int preloadInt;
		public bool ae;
		// Update every frame
		void Timer1Tick(object sender, EventArgs e)
		{
			if(process){
				if(!prelisted){
					// Check if the process is finished
					if(currentFolder < foundFolders.Length - 1){
						// Finding files and filter osu files
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
							// Get map info
							OsuReader.OsuMap mapInfo = OsuReader.GetOsuMapInfo(foundOsuFiles[0]);
							string audioFile = Path.Combine(dir, mapInfo.AudioFile);
							string endFile = Path.Combine(fetchedSongsFolder, mapInfo.Artist + " - " + mapInfo.Title + Path.GetExtension(audioFile));
							string secondAttempt = Path.Combine(fetchedSongsFolder, RemoveInitialNumbers(GetFolderName(dir)) + Path.GetExtension(audioFile));
							string result = "Failed...";
							if(File.Exists(audioFile)){
								// Attempt 1 : Naming audio as artist and the audio title
								try {
									File.WriteAllBytes(endFile, File.ReadAllBytes(audioFile));
									result = "Success!";
								} catch (Exception ea) {
									Log("First attempt failed : " + ea.Message);
									result = "First attempt failed : " + ea.Message;
								}
								// Attempt 2 : Naming audio as the folder name
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
							// Adding result to the list
							mapInfo.order = currentFolder;
							ListViewItem osuFile = AddItem(mapInfo.Title,mapInfo.Artist, dir, result);
							maps.Add(mapInfo);
							mapsListed.Add(osuFile);
						}
						currentFolder++;
					}else{
						// Ae is to prevent infinite calls
						if(!ae){
							Log("Fetching Done!");
							ae = false;
						}
						process = false;
					}
				} else{
					// Listing algorithm, similar to fetching but if the list have been prelisted
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
						currentFolder++;
						if(!ae){
							Log("Fetching Done!");
							ae = true;
						}
						process = false;
					}
				}
				progressBar1.Value = Mathf.Clamp(currentFolder, 0, progressBar1.Maximum);
				progressBar1.Maximum = foundFolders.Length - 1;
				label3.Text = currentFolder.ToString() + "/" + (foundFolders.Length - 1).ToString();
			}
			if(processList){
				// Also similar from listing algorithm with the fetch.
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
				progressBar1.Value = Mathf.Clamp(currentFolder, 0, progressBar1.Maximum);
				progressBar1.Maximum = foundFolders.Length - 1;
				label3.Text = currentFolder.ToString() + "/" + (foundFolders.Length - 1).ToString();
			}
		}
		/// <summary>
		/// Adding list to list view with sub items
		/// </summary>
		/// <param name="title">Audio Title</param>
		/// <param name="artist">Audio Artist</param>
		/// <param name="folder">Song Folder</param>
		/// <param name="status">Fetch Status</param>
		/// <returns></returns>
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
