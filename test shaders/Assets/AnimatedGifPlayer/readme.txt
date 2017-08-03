Animated GIF Player

Main features:

-Plays animated GIFS on any texture.
-Can load GIFs from local storage or web
-Easy to use
-GIFs are decoded in a seperate thread for performance
-Caching of animated frames
-Examples included


Updates:
V1.12
	-Gifs can now be loaded from: Application.streamingAssetsPath (default), Application.persistentDataPath, Application.temporaryCachePath as well as http and https.
V1.11
	-Added controls for playback speed under advanced options
V1.1
	-Changed namespace to OldMoatGames. If you are updating from 1.0 change "using AnimatedGifPlayer;" to "using OldMoatGames;" in your code
	-Can load GIFs from web instead of just StreamingAssetsFolder. Only usable from code. See the code example scene for more info 
	-Fixed the duration of the visibility of the first frame when resuming playback with the Play() method
	-Added OnLoadError event
	-Added Width and Height vars that return the size of the GIF once loaded
V1.02
	-Fixed playback on WebGL
V1.01
	-Bug fixes
V1.0
	-First release