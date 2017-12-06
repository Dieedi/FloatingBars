using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FloatingBarsCreateWindow : EditorWindow
{
	// Mandatory Fields
	public static GameObject targettedGameObject;
	public static FloatVariable resourceVariable;
	public static FloatVariable minResourceVariable;
	public static FloatVariable maxResourceVariable;

	// Optional Fields
	public static FloatVariable regenRate;
	public static float posX = 0;
	public static float posY = 2f;
	public static float posZ = 0;
	public static Color positiveColor = Color.green;
	public static Color negativeColor = Color.red;
	public static Sprite positiveImage;
	public static Sprite negativeImage;
	static string titleWindow = "ie : Health Points";

	// Temporary save datas
	public static FloatVariable tmp_resourceVariable;
	public static FloatVariable tmp_minResourceVariable;
	public static FloatVariable tmp_maxResourceVariable;
	public static FloatVariable tmp_regenRate;
	public static float tmp_posX;
	public static float tmp_posY;
	public static float tmp_posZ;
	public static Color tmp_positiveColor;
	public static Color tmp_negativeColor;
	public static Sprite tmp_positiveImage;
	public static Sprite tmp_negativeImage;
	static string tmp_titleWindow;

	// Create FloatVariable Menu default values
	bool groupEnabled;
	static bool isOnEditMode = false;
	bool showCreateResource = false;
	bool showCreateMinResource = false;
	bool showCreateMaxResource = false;
	bool showCreateRegenRate = false;
	string floatVariableName = "new FloatVariable";
	float floatVariableValue = 0f;
	Vector2 scrollPos;


	//============================
	// Create Bar Window
	//============================
	[MenuItem("Floating Bars/Open Creator", false, 1)]
	private static void ShowWindow()
	{
		positiveImage = Resources.Load<Sprite>("FloatingBar");
		negativeImage = Resources.Load<Sprite>("FloatingBar");
		FloatingBarsCreateWindow window = (FloatingBarsCreateWindow)GetWindow(typeof(FloatingBarsCreateWindow));
		window.minSize = new Vector2(400, 400);
		window.titleContent = new GUIContent("Create Bar");
		window.Show();
	}

	//============================
	// Edit Bar Window
	//============================

	[MenuItem("Floating Bars/Edit Bar (Bar has to be selected)", false, 2)]
	private static void ShowEditWindow()
	{
		SaveDatas();

		isOnEditMode = true;

		FloatingBarsCreateWindow window = (FloatingBarsCreateWindow)GetWindow(typeof(FloatingBarsCreateWindow));
		window.minSize = new Vector2(400, 400);
		window.titleContent = new GUIContent("Edit Bar");
		window.Show();
	}

	// Validate that the object can receive a floating bar
	[MenuItem("Floating Bars/Edit Bar (Bar has to be selected)", true)]
	private static bool ShowEditWindowValidation()
	{
		// Return true if we select a gameObject
		if (Selection.activeGameObject.GetComponent<FloatingBarController>())
			return Selection.activeGameObject.GetComponent<FloatingBarController>().GetType() == typeof(FloatingBarController);

		return false;
	}

	//==============================
	// Destroy Item
	//==============================

	[MenuItem("Floating Bars/Destroy Bar (Bar has to be selected)", false, 2)]
	private static void DestroyBar()
	{
		DestroyImmediate(Selection.activeGameObject.GetComponentInParent<FloatingBarVariableController>());
		DestroyImmediate(Selection.activeGameObject);
	}

	// Validate that the object can receive a floating bar
	[MenuItem("Floating Bars/Destroy Bar (Bar has to be selected)", true)]
	private static bool DestroyBarValidation()
	{
		// Return true if we select a gameObject
		if (Selection.activeGameObject.GetComponent<FloatingBarController>())
			return Selection.activeGameObject.GetComponent<FloatingBarController>().GetType() == typeof(FloatingBarController);

		return false;
	}

	//===========================
	// Add Window Content
	//===========================
	private void OnGUI()
	{
		CreateWindowLayout();

		if (targettedGameObject == null
			|| resourceVariable == null || minResourceVariable == null || maxResourceVariable == null)
			GUI.enabled = false;

		if (!isOnEditMode && GUILayout.Button("Add New Floating Bar")) {
			AddGeneratedBar();
		}

		if (isOnEditMode) {
			if (GUILayout.Button("Reset"))
				RestoreDatas();

			if (GUILayout.Button("Save Changes & Close Edit"))
				ApplyChanges();
		}
	}

	//========================================
	// Add new bar and main object connector
	//========================================
	private void AddGeneratedBar()
	{
		// Load Floating Bar Prefab and add it to target
		GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/FloatingBars/Prefabs/FloatingBar Container.prefab", typeof(GameObject)) as GameObject;
		GameObject floatingBarPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

		// Modify elements with given values
		floatingBarPrefab.name = titleWindow;

		FloatingBarController floatingBarController = floatingBarPrefab.GetComponent<FloatingBarController>();
		floatingBarController.resource = resourceVariable;
		floatingBarController.Min = minResourceVariable;
		floatingBarController.Max = maxResourceVariable;

		Image backgroundImage = floatingBarPrefab.GetComponentsInChildren<Image>()[0];
		backgroundImage.color = negativeColor;
		backgroundImage.sprite = positiveImage;

		Image foregroundImage = floatingBarPrefab.GetComponentsInChildren<Image>()[1];
		foregroundImage.color = positiveColor;
		foregroundImage.sprite = negativeImage;

		floatingBarPrefab.transform.SetParent(targettedGameObject.transform);

		// Change local position after assigning to parent to avoid misplacement
		floatingBarPrefab.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, posZ);

		// Add floating bar variable controller
		FloatingBarVariableController fbv_controller =  targettedGameObject.AddComponent<FloatingBarVariableController>();
		fbv_controller.currentValue = resourceVariable;
		fbv_controller.resetValue = true; // default to true ... to define
		fbv_controller.startingValue = maxResourceVariable;
		fbv_controller.RegenRate = regenRate;
	}

	//=================================
	// Add loaded datas to temp values
	//=================================
	private static void SaveDatas()
	{
		// get values from object
		GameObject activeGO = Selection.activeGameObject;
		tmp_titleWindow = titleWindow = activeGO.name;
		FloatingBarVariableController activeGO_parentFbvc = activeGO.GetComponentInParent<FloatingBarVariableController>();
		targettedGameObject = activeGO_parentFbvc.gameObject;
		FloatingBarController activeGO_fbc = activeGO.GetComponent<FloatingBarController>();
		tmp_resourceVariable = resourceVariable = activeGO_fbc.resource;
		tmp_minResourceVariable = minResourceVariable = activeGO_fbc.Min;
		tmp_maxResourceVariable = maxResourceVariable = activeGO_fbc.Max;
		tmp_regenRate = regenRate = activeGO_parentFbvc.RegenRate;
		RectTransform activeGO_transform = activeGO_fbc.GetComponent<RectTransform>();
		tmp_posX = posX = activeGO_transform.position.x;
		tmp_posY = posY = activeGO_transform.position.y;
		tmp_posZ = posZ = activeGO_transform.position.z;
		Image activeGO_fgdImage = activeGO.GetComponentsInChildren<Image>()[0];
		Image activeGO_bgdImage = activeGO.GetComponentsInChildren<Image>()[1];
		tmp_positiveColor = positiveColor = activeGO_bgdImage.color;
		tmp_negativeColor = negativeColor = activeGO_fgdImage.color;
		tmp_positiveImage = positiveImage = activeGO_bgdImage.sprite;
		tmp_negativeImage = negativeImage = activeGO_fgdImage.sprite;
	}

	//======================================
	// Restore stored data from temp values
	//======================================
	private void RestoreDatas()
	{
		GameObject activeGO = Selection.activeGameObject;
		titleWindow = tmp_titleWindow;
		FloatingBarVariableController activeGO_parentFbvc = activeGO.GetComponentInParent<FloatingBarVariableController>();
		FloatingBarController activeGO_fbc = activeGO.GetComponent<FloatingBarController>();
		activeGO_fbc.resource = resourceVariable = tmp_resourceVariable;
		activeGO_fbc.Min = minResourceVariable = tmp_minResourceVariable;
		activeGO_fbc.Max = maxResourceVariable = tmp_maxResourceVariable;
		activeGO_parentFbvc.RegenRate = regenRate = tmp_regenRate;
		RectTransform activeGO_transform = activeGO_fbc.GetComponent<RectTransform>();

		// We can't change x, y, z values alone
		activeGO_transform.position = new Vector3(tmp_posX, tmp_posY, tmp_posZ);
		posX = tmp_posX;
		posY = tmp_posY;
		posZ = tmp_posZ;
		Image activeGO_fgdImage = activeGO.GetComponentsInChildren<Image>()[0];
		Image activeGO_bgdImage = activeGO.GetComponentsInChildren<Image>()[1];
		activeGO_bgdImage.color = positiveColor = tmp_positiveColor;
		activeGO_fgdImage.color = negativeColor = tmp_negativeColor;
		activeGO_bgdImage.sprite = positiveImage = tmp_positiveImage;
		activeGO_fgdImage.sprite = negativeImage = tmp_negativeImage;
	}

	//===================================================
	// Only Close window, values are immediately applied
	//===================================================
	private void ApplyChanges()
	{
		isOnEditMode = false;
		FloatingBarsCreateWindow window = (FloatingBarsCreateWindow)GetWindow(typeof(FloatingBarsCreateWindow));
		window.Close();
	}

	//================================
	// Create the whole window layout
	//================================
	private void CreateWindowLayout()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		// Take care of EditMode to check modification (not needed on create as we don't see the bar)
		if (isOnEditMode) {
			EditorGUI.BeginChangeCheck();
			GUILayout.Label("Edit " + titleWindow + " Bar", EditorStyles.boldLabel);
		} else {
			GUILayout.Label("Create New Bar", EditorStyles.boldLabel);
		}
		titleWindow = EditorGUILayout.TextField("Floating Bar Name : ", titleWindow);

		GUILayout.Space(10f);

		GUILayout.Label("Mandatory Settings :", EditorStyles.label);

		targettedGameObject = EditorGUILayout.ObjectField(
			"Attach to : ",
			targettedGameObject,
			typeof(GameObject),
			true) as GameObject;

		resourceVariable = EditorGUILayout.ObjectField(
			"Resource Variable : ",
			resourceVariable,
			typeof(FloatVariable),
			true) as FloatVariable;

		// Add foldout to allow 'on the fly' scriptableobject creation. Object created is immediately added to field
		showCreateResource = EditorGUILayout.Foldout(showCreateResource, "Add new FloatVariable");
		if (showCreateResource) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				resourceVariable = CreateNewFloatVariable();
				showCreateResource = !showCreateResource;
			}
		}

		minResourceVariable = EditorGUILayout.ObjectField(
			"MinResource Variable : ",
			minResourceVariable,
			typeof(FloatVariable),
			true) as FloatVariable;

		// Add foldout to allow 'on the fly' scriptableobject creation. Object created is immediately added to field
		showCreateMinResource = EditorGUILayout.Foldout(showCreateMinResource, "Add new FloatVariable");
		if (showCreateMinResource) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				minResourceVariable = CreateNewFloatVariable();
				showCreateMinResource = !showCreateMinResource;
			}
		}

		maxResourceVariable = EditorGUILayout.ObjectField(
			"MaxResource Variable : ",
			maxResourceVariable,
			typeof(FloatVariable),
			true) as FloatVariable;

		// Add foldout to allow 'on the fly' scriptableobject creation. Object created is immediately added to field
		showCreateMaxResource = EditorGUILayout.Foldout(showCreateMaxResource, "Add new FloatVariable");
		if (showCreateMaxResource) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				maxResourceVariable = CreateNewFloatVariable();
				showCreateMaxResource = !showCreateMaxResource;
			}
		}

		regenRate = EditorGUILayout.ObjectField(
			"RegenRate Variable : ",
			regenRate,
			typeof(FloatVariable),
			true) as FloatVariable;

		// Add foldout to allow 'on the fly' scriptableobject creation. Object created is immediately added to field
		showCreateRegenRate = EditorGUILayout.Foldout(showCreateRegenRate, "Add new FloatVariable");
		if (showCreateRegenRate) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				regenRate = CreateNewFloatVariable();
				showCreateRegenRate = !showCreateRegenRate;
			}
		}

		GUILayout.Space(10f);

		GUILayout.Label("Optional Settings : (Default values will be applied)", EditorStyles.label);

		posX = EditorGUILayout.FloatField("Position X :", posX);
		posY = EditorGUILayout.FloatField("Position Y :", posY);
		posZ = EditorGUILayout.FloatField("Position Z :", posZ);
		positiveColor = EditorGUILayout.ColorField("Positive Color :", positiveColor);
		negativeColor = EditorGUILayout.ColorField("Negative Color :", negativeColor);

		positiveImage = EditorGUILayout.ObjectField(
			"Image for Positive Bar :",
			positiveImage,
			typeof(Sprite),
			true) as Sprite;
		negativeImage = EditorGUILayout.ObjectField(
			"Image for Positive Bar :",
			negativeImage,
			typeof(Sprite),
			true) as Sprite;

		// Only in edit mode, check changes and apply them
		if (isOnEditMode && EditorGUI.EndChangeCheck()) {
			ApplyModifications();
		}
		EditorGUILayout.EndScrollView();
	}

	//=====================================================
	// Only for EditMode, 'immediately' apply changes made
	//=====================================================
	private void ApplyModifications ()
	{
		GameObject activeGO = Selection.activeGameObject;
		activeGO.name = titleWindow;
		FloatingBarVariableController activeGO_parentFbvc = activeGO.GetComponentInParent<FloatingBarVariableController>();
		FloatingBarController activeGO_fbc = activeGO.GetComponent<FloatingBarController>();
		activeGO_fbc.resource = resourceVariable;
		activeGO_fbc.Min = minResourceVariable;
		activeGO_fbc.Max = maxResourceVariable;
		activeGO_parentFbvc.RegenRate = regenRate;
		RectTransform activeGO_transform = activeGO_fbc.GetComponent<RectTransform>();
		activeGO_transform.position = new Vector3(posX, posY, posZ);
		Image activeGO_fgdImage = activeGO.GetComponentsInChildren<Image>()[0];
		Image activeGO_bgdImage = activeGO.GetComponentsInChildren<Image>()[1];
		activeGO_bgdImage.color = positiveColor;
		activeGO_fgdImage.color = negativeColor;
		activeGO_bgdImage.sprite = positiveImage;
		activeGO_fgdImage.sprite = negativeImage;
	}

	//==========================================
	// FloatVariable (ScriptableObject) creator
	//==========================================
	private FloatVariable CreateNewFloatVariable()
	{
		string path = "Assets/FloatingBars/ScriptableObjects Example";
		FloatVariable newFloatVariable = CreateInstance("FloatVariable") as FloatVariable;
		newFloatVariable.name = floatVariableName;
		newFloatVariable.SetValue(floatVariableValue);

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + floatVariableName + ".asset");
		AssetDatabase.CreateAsset(newFloatVariable, assetPathAndName);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		return newFloatVariable;
	}
}
