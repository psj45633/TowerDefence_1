using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;   // 닫을 패널 최상위
    [SerializeField] private Transform ownerRoot;    // 타워 루트(자식 포함 클릭 허용)
    [SerializeField] private LayerMask ownerLayers = ~0;

    Camera cam;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!panelRoot || !panelRoot.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ShouldClose(Input.mousePosition))
                panelRoot.SetActive(false);
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began && ShouldClose(t.position))
                panelRoot.SetActive(false);
        }
    }

    bool ShouldClose(Vector2 screenPos)
    {
        // 1) 패널 내부(UI 버튼 포함)라면 닫지 않음
        if (IsPointerOverPanelUI(screenPos)) return false;

        // 2) 오너(타워) 본체/자식이라면 닫지 않음
        if (IsPointerOverOwner(screenPos)) return false;

        // 3) 그 외엔 닫기
        return true;
    }

    // 씬의 모든 GraphicRaycaster를 통해 UI 히트 → panelRoot 내부인지 확인
    bool IsPointerOverPanelUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        var ped = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results); // 중요: 특정 Raycaster 참조 대신 전역 레이캐스트

        foreach (var r in results)
        {
            var tr = r.gameObject.transform;
            if (tr == panelRoot.transform || tr.IsChildOf(panelRoot.transform))
                return true; // 패널 내부(UI 버튼/텍스트/이미지 등) 클릭
        }
        return false;
    }

    bool IsPointerOverOwner(Vector2 screenPos)
    {
        if (!ownerRoot || cam == null) return false;
        var world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        var hits = Physics2D.OverlapPointAll((Vector2)world, ownerLayers);
        for (int i = 0; i < hits.Length; i++)
        {
            var t = hits[i].transform;
            if (!t) continue;
            if (t == ownerRoot || t.IsChildOf(ownerRoot)) return true;
        }
        return false;
    }
}