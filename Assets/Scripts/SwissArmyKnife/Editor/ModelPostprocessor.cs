using UnityEngine;
using UnityEditor;

namespace SwissArmyKnife
{
	namespace Editor
	{
		public class ModelPostprocessor : AssetPostprocessor {

			private void OnPreprocessModel()
			{
				ModelImporter modelImporter = assetImporter as ModelImporter;
				modelImporter.globalScale = 1.0f;
			}

			private void OnPostprocessModel(GameObject gameObject)
			{}
		}
	}
}