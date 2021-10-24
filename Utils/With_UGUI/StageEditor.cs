using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Stage))]
public class StageEditor : Editor
{
    Stage stageObject = null;

    private void OnEnable()
    {
        stageObject = (Stage)target;      
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("스테이지 정보 가져오기"))
        {
            stageObject.sceneList.Clear();

            Scene curScene = SceneManager.GetActiveScene();
            foreach (var room in curScene.GetRootGameObjects())
            {
                if (room.GetComponent<StageScene>() != null)
                {
                    stageObject.sceneList.Add(room.GetComponent<StageScene>());
                }
            }
        }
        
        if (GUILayout.Button("현 스테이지 정보를 엑셀로 저장하기"))
        {
            // Stage(.cs)의 Scene List를 스캔한다.
            stageObject.sceneList.Clear();

            Scene curScene = SceneManager.GetActiveScene();
            foreach (var room in curScene.GetRootGameObjects())
            {
                if (room.GetComponent<StageScene>() != null)
                {
                    stageObject.sceneList.Add(room.GetComponent<StageScene>());
                }
            }

            bool sameNameCheck = false;
            // 각 Stage Scene(.cs)의 Trigger Object List를 스캔한다.
            for (int i = 0; i < stageObject.sceneList.Count; i++)
            {
                stageObject.sceneList[i].ScanTriggerObject();
                if (stageObject.sceneList[i].sameNameCount > 0)
                    sameNameCheck = true;
            }
            if (!sameNameCheck)
            {
                StageToExcelWriter excelWriter = new StageToExcelWriter();
                excelWriter.ConvertToExcel(stageObject);
                EditorUtility.DisplayDialog("엑셀 쓰기", "스테이지 데이터를 엑셀 파일로 변환했습니다.", "알겠습니다");
            }
            else
            {
                EditorUtility.DisplayDialog("엑셀 쓰기", "중복 오브젝트 이름이 발견되어 엑셀 변환을 실패했습니다.", "알겠습니다");
            }
        }

        if (GUILayout.Button("엑셀 파일을 이 스테이지에 내려받아 세팅"))
        {
            string path = EditorUtility.OpenFilePanel("가져올 엑셀 파일을 선택해주세요", RootPathForStageExcel, "xls");
            if (path.Length != 0)
            {
                ExcelToStageWriter writer = new ExcelToStageWriter();
                string message = "";

                message = writer.SyncStageWithExcel(ref stageObject, path);
                if (message != null)
                {
                    EditorUtility.DisplayDialog("엑셀 쓰기", message, "알겠습니다.");
                }
                else
                {
                    EditorUtility.DisplayDialog("엑셀 쓰기", "성공적으로 동기화했습니다.", "알겠습니다.");
                }

                Debug.Log(path);
            }
        }
    }
    const string RootPathForStageExcel = "Assets/Resources/ExcelData";

}