//using UnityEditor;
//
//class MyEditorScript
//{
//	static void PerformBuild ()
//	{
//		string[] scenes = { "Scenes/UI/menuScene.unity", "Scenes/Level1/townLevel1Scene" };
//		string buildPath = "/Users/nm/loom-path/src/github.com/loomnetwork/webgl";
//		BuildOptions opt = BuildOptions.None;
//		BuildTarget target = BuildTarget.WebGL;
//		BuildPipeline.BuildPlayer (scenes, buildPath, target, opt);
////		BuildPipeline.BuildPlayer(scenes, ...);
//	}
//}

using UnityEditor;
class WebGLBuilder {
	static void build() {

		// Place all your scenes here
		string[] scenes = {
			"Assets/Scenes/UI/menuScene.unity", 
			"Assets/Scenes/Level1/townLevel1Scene.unity",
			"Assets/Scenes/Level1/innLevel1Scene.unity",
			"Assets/Scenes/Level1/forestLevel1Scene.unity",
			"Assets/Scenes/Level1/privateRoom1Scene.unity",
			"Assets/Scenes/Level2/townLevel2Scene.unity",
			"Assets/Scenes/Level2/innLevel2Scene.unity",
			"Assets/Scenes/Level2/privateRoom2Scene.unity",
			"Assets/Scenes/Level2/forestLevel2Scene.unity",
			"Assets/Scenes/Level3/townLevel3Scene.unity",
			"Assets/Scenes/Level3/innLevel3Scene.unity",
			"Assets/Scenes/Level3/forestLevel3Scene.unity",
			"Assets/Scenes/Level3/privateRoom3Scene.unity",
			"Assets/Scenes/innerTemple/innerTempleScene.unity",
			"Assets/Scenes/darkLevel3/darkForestLevel3Scene.unity",
			"Assets/Scenes/darkLevel3/darkTownLevel3Scene.unity",
			"Assets/Scenes/darkLevel2/darkForestLevel2Scene.unity",
			"Assets/Scenes/darkLevel2/darkTownLevel2Scene.unity",
			"Assets/Scenes/darkLevel1/darkForestLevel1Scene.unity",
			"Assets/Scenes/darkLevel1/darkTownLevel1Scene.unity",
			"Assets/Scenes/darkLevel3/darkInnLevel3Scene.unity",
			"Assets/Scenes/darkLevel3/darkPrivateRoom3Scene.unity",
			"Assets/Scenes/darkLevel2/darkInnLevel2Scene.unity",
			"Assets/Scenes/darkLevel2/darkPrivateRoom2Scene.unity",
			"Assets/Scenes/UI/gameOverScene.unity",
			"Assets/Scenes/UI/uploadedScene.unity",
			"Assets/Scenes/UI/creditsScene.unity",
			"Assets/Scenes/Level1/townLevel1CutScene.unity",
			"Assets/Scenes/Level1/privateRoomCutScene.unity",
			"Assets/Scenes/Level1/townLevel1CutScene2.unity",
		};

		string pathToDeploy = "builds/WebGLversion/";       

		BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);      
	}
}