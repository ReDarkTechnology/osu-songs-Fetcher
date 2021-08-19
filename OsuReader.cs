using System;
using System.IO;

public static class OsuReader {
	[Serializable]
	public class OsuMap {
		public string Title;
		public string Artist;
		public string AudioFile;
	}
	public static OsuMap GetOsuMapInfo(string osuFile){
		bool resultTaken = false;
		int informationsTaken = 0;
		var map = new OsuMap();
		using (StreamReader reader = new StreamReader(osuFile)) {
			while(!resultTaken){
				try {
					string result = reader.ReadLine();
					if(result.StartsWith("AudioFilename: ", StringComparison.CurrentCulture)){
						map.AudioFile = result.Remove(0,15);
						informationsTaken++;
					}
					if(result.StartsWith("Title:", StringComparison.CurrentCulture)){
						map.Title = result.Remove(0,6);
						informationsTaken++;
					}
					if(result.StartsWith("Artist:", StringComparison.CurrentCulture)){
						map.Artist = result.Remove(0,7);
						informationsTaken++;
					}
					if(informationsTaken > 2){
						resultTaken = true;
					}
				} catch {
					resultTaken = true;
				}
			}
		}
		return map;
	}
}