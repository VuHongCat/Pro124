using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterIndexUI : MonoBehaviour
{
    public static MonsterIndexUI Instance;

    private GameObject panelRoot;
    private GameObject listRoot;
    private GameObject detailRoot;
    private Text indexButtonLabel;
    private Sprite placeholderSprite;
    private readonly List<GameObject> cells = new();

    private MonsterCategory? currentFilter;
    private int page;
    private const int PageSize = 15;

    private static readonly Color NormalFrame = new Color(0.16f, 0.16f, 0.22f, 1f);
    private static readonly Color MiniFrame = new Color(0.42f, 0.22f, 0.62f, 1f);
    private static readonly Color BossFrame = new Color(0.62f, 0.18f, 0.18f, 1f);
    private static readonly Color DarkFrame = new Color(0.12f, 0.12f, 0.16f, 1f);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void BuildUI()
    {
        Canvas canvas = RuntimeUi.CreateCanvas("MonsterIndexCanvas");
        canvas.sortingOrder = 170;

        GameObject btnGo = new GameObject("IndexButton", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform btnRt = btnGo.GetComponent<RectTransform>();
        btnRt.SetParent(canvas.transform, false);
        btnRt.anchorMin = new Vector2(0, 0);
        btnRt.anchorMax = new Vector2(0, 0);
        btnRt.pivot = new Vector2(0, 0);
        btnRt.anchoredPosition = new Vector2(8, 108);
        btnRt.sizeDelta = new Vector2(200, 42);

        Image img = btnGo.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        Button btn = btnGo.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(TogglePanel);

        indexButtonLabel = RuntimeUi.CreateText(btnRt, "Monster Index", 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

        panelRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.92f));
        panelRoot.SetActive(false);

        listRoot = RuntimeUi.CreatePanel(panelRoot.transform, new Color(0, 0, 0, 0f));
        detailRoot = RuntimeUi.CreatePanel(panelRoot.transform, new Color(0, 0, 0, 0f));
        detailRoot.SetActive(false);
    }

    private void TogglePanel()
    {
        if (panelRoot.activeSelf)
        {
            panelRoot.SetActive(false);
            return;
        }

        listRoot.SetActive(true);
        detailRoot.SetActive(false);
        Refresh();
        panelRoot.SetActive(true);
    }

    private List<MonsterCatalogEntry> GetFiltered()
    {
        List<MonsterCatalogEntry> all = RuntimeEnemyLibrary.GetMonsterCatalog();
        if (currentFilter == null) return all;
        return all.FindAll(e => e.category == currentFilter.Value);
    }

    private void Refresh()
    {
        if (listRoot == null) return;

        foreach (Transform child in listRoot.transform)
            Destroy(child.gameObject);
        cells.Clear();

        List<MonsterCatalogEntry> entries = GetFiltered();

        RuntimeUi.CreateText(listRoot.transform, "Monster Index", 26, TextAnchor.UpperCenter,
            new Vector2(0, 0.9f), new Vector2(1, 1));

        if (indexButtonLabel != null)
            indexButtonLabel.text = $"Monster Index ({entries.Count})";

        CreateFilterButtons();

        int totalPages = Mathf.Max(1, Mathf.CeilToInt(entries.Count / (float)PageSize));
        page = Mathf.Clamp(page, 0, totalPages - 1);

        int start = page * PageSize;
        int end = Mathf.Min(start + PageSize, entries.Count);

        if (entries.Count == 0)
        {
            RuntimeUi.CreateText(listRoot.transform, "No monsters found", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.55f), new Vector2(1, 0.7f));
        }
        else
        {
            BuildCells(entries, start, end);
        }

        CreatePageControls(totalPages);
        CreateCloseButton();
    }

    private void CreateFilterButtons()
    {
        string[] labels = { "All", "Normal", "Mini Boss", "Boss" };
        MonsterCategory?[] filters = { null, MonsterCategory.Normal, MonsterCategory.MiniBoss, MonsterCategory.Boss };

        for (int i = 0; i < labels.Length; i++)
        {
            MonsterCategory? filter = filters[i];
            bool active = currentFilter == filter;
            GameObject go = new GameObject("Filter_" + labels[i], typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(listRoot.transform, false);
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(-248 + i * 130, -66);
            rt.sizeDelta = new Vector2(122, 40);

            Image img = go.GetComponent<Image>();
            img.color = active ? new Color(0.5f, 0.5f, 0.55f, 1f) : new Color(0.15f, 0.15f, 0.2f, 1f);
            Button btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() =>
            {
                currentFilter = filter;
                page = 0;
                Refresh();
            });

            RuntimeUi.CreateText(rt, labels[i], 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }
    }

    private void BuildCells(List<MonsterCatalogEntry> entries, int start, int end)
    {
        const int cols = 5;
        const float cellW = 175f;
        const float cellH = 200f;

        for (int i = start; i < end; i++)
        {
            MonsterCatalogEntry entry = entries[i];
            if (entry == null || entry.data == null) continue;

            int index = i - start;
            int row = index / cols;
            int col = index % cols;
            float x = (col - (cols - 1) * 0.5f) * cellW;
            float y = -150f - row * cellH;

            GameObject cell = new GameObject("MonsterCell", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = cell.GetComponent<RectTransform>();
            rt.SetParent(listRoot.transform, false);
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(155, 190);

            Image frame = cell.GetComponent<Image>();
            frame.color = GetFrameColor(entry.category);
            Button cellBtn = cell.GetComponent<Button>();
            cellBtn.targetGraphic = frame;
            MonsterCatalogEntry captured = entry;
            cellBtn.onClick.AddListener(() => ShowDetail(captured));

            CreateCategoryBadge(rt, entry.category);

            CreateMapsBadge(rt, entry.maps);

            GameObject artGo = new GameObject("Art", typeof(RectTransform), typeof(Image));
            RectTransform artRt = artGo.GetComponent<RectTransform>();
            artRt.SetParent(rt, false);
            artRt.anchorMin = new Vector2(0.5f, 1f);
            artRt.anchorMax = new Vector2(0.5f, 1f);
            artRt.pivot = new Vector2(0.5f, 1f);
            artRt.anchoredPosition = new Vector2(0, -6);
            artRt.sizeDelta = new Vector2(143, 140);
            Image art = artGo.GetComponent<Image>();
            if (entry.data.artwork != null)
            {
                art.sprite = entry.data.artwork;
                art.color = Color.white;
            }
            else
            {
                art.sprite = GetPlaceholder();
                art.color = new Color(0.25f, 0.25f, 0.3f, 1f);

                string initial = entry.data.enemyName.Length > 0 ? entry.data.enemyName[0].ToString() : "?";
                Text artLabel = RuntimeUi.CreateText(artRt, initial, 64, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
                artLabel.color = new Color(1f, 1f, 1f, 0.18f);
            }

            GameObject nameBar = new GameObject("NameBar", typeof(RectTransform), typeof(Image));
            RectTransform nameRt = nameBar.GetComponent<RectTransform>();
            nameRt.SetParent(rt, false);
            nameRt.anchorMin = new Vector2(0, 0);
            nameRt.anchorMax = new Vector2(1, 0.16f);
            nameRt.offsetMin = Vector2.zero;
            nameRt.offsetMax = Vector2.zero;
            nameBar.GetComponent<Image>().color = DarkFrame;

            Text name = RuntimeUi.CreateText(nameRt, entry.data.enemyName, 12, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            name.rectTransform.anchorMin = Vector2.zero;
            name.rectTransform.anchorMax = Vector2.one;
            name.rectTransform.offsetMin = new Vector2(2, 0);
            name.rectTransform.offsetMax = new Vector2(-2, 0);
            name.horizontalOverflow = HorizontalWrapMode.Wrap;

            cells.Add(cell);
        }
    }

    private void CreateCategoryBadge(RectTransform parent, MonsterCategory category)
    {
        GameObject badge = new GameObject("Category", typeof(RectTransform), typeof(Image));
        RectTransform rt = badge.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(16, -14);
        rt.sizeDelta = new Vector2(28, 28);
        Image bg = badge.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        string letter = category == MonsterCategory.Normal ? "N"
            : category == MonsterCategory.MiniBoss ? "M" : "B";
        Text t = RuntimeUi.CreateText(rt, letter, 14, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        t.color = GetFrameColor(category);
    }

    private void CreateMapsBadge(RectTransform parent, List<int> maps)
    {
        string label = maps.Count > 0 ? "Map " + string.Join(",", maps) : "";
        if (label.Length == 0) return;

        GameObject badge = new GameObject("Maps", typeof(RectTransform), typeof(Image));
        RectTransform rt = badge.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(-16, -14);
        rt.sizeDelta = new Vector2(64, 24);
        Image bg = badge.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        RuntimeUi.CreateText(rt, label, 10, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
    }

    private void CreatePageControls(int totalPages)
    {
        Text pageText = RuntimeUi.CreateText(listRoot.transform, $"Page {page + 1} / {totalPages}", 16, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        RectTransform pt = pageText.rectTransform;
        pt.anchorMin = new Vector2(0.5f, 1f);
        pt.anchorMax = new Vector2(0.5f, 1f);
        pt.pivot = new Vector2(0.5f, 1f);
        pt.anchoredPosition = new Vector2(0, -800);
        pt.sizeDelta = new Vector2(200, 40);

        bool hasPrev = page > 0;
        bool hasNext = page < totalPages - 1;

        CreateTopButton("Prev", new Vector2(-160, -800), new Vector2(120, 42), () => { page--; Refresh(); }, hasPrev);
        CreateTopButton("Next", new Vector2(160, -800), new Vector2(120, 42), () => { page++; Refresh(); }, hasNext);
    }

    private void CreateCloseButton()
    {
        CreateTopButton("Close", new Vector2(310, -800), new Vector2(140, 42), TogglePanel, true);
    }

    private void CreateTopButton(string label, Vector2 pos, Vector2 size, System.Action onClick, bool interactable)
    {
        GameObject go = new GameObject("Btn_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(listRoot.transform, false);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.interactable = interactable;
        if (interactable)
            btn.onClick.AddListener(() => onClick());
        if (!interactable)
        {
            ColorBlock cb = btn.colors;
            cb.disabledColor = new Color(0.45f, 0.45f, 0.5f, 0.7f);
            btn.colors = cb;
        }

        RuntimeUi.CreateText(rt, label, 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
    }

    // =========================================================
    // DETAIL VIEW (full-screen, "scene-like")
    // =========================================================

    private void ShowDetail(MonsterCatalogEntry entry)
    {
        if (entry == null || entry.data == null) return;

        listRoot.SetActive(false);
        detailRoot.SetActive(true);

        foreach (Transform child in detailRoot.transform)
            Destroy(child.gameObject);

        EnemyData d = entry.data;

        GameObject artGo = new GameObject("DetailArt", typeof(RectTransform), typeof(Image));
        RectTransform artRt = artGo.GetComponent<RectTransform>();
        artRt.SetParent(detailRoot.transform, false);
        artRt.anchorMin = new Vector2(0.5f, 0.5f);
        artRt.anchorMax = new Vector2(0.5f, 0.5f);
        artRt.pivot = new Vector2(0.5f, 0.5f);
        artRt.anchoredPosition = new Vector2(0, 280);
        artRt.sizeDelta = new Vector2(240, 240);
        Image art = artGo.GetComponent<Image>();
        if (d.artwork != null)
        {
            art.sprite = d.artwork;
            art.color = Color.white;
        }
        else
        {
            art.sprite = GetPlaceholder();
            art.color = new Color(0.25f, 0.25f, 0.3f, 1f);

            string initial = d.enemyName.Length > 0 ? d.enemyName[0].ToString() : "?";
            Text artLabel = RuntimeUi.CreateText(artRt, initial, 110, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            artLabel.color = new Color(1f, 1f, 1f, 0.18f);
        }

        string maps = entry.maps.Count > 0 ? string.Join(", ", entry.maps) : "-";

        Text nameText = RuntimeUi.CreateText(detailRoot.transform, d.enemyName, 30, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        RectTransform nrt = nameText.rectTransform;
        nrt.pivot = new Vector2(0.5f, 0.5f);
        nrt.anchoredPosition = new Vector2(0, 140);
        nrt.sizeDelta = new Vector2(900, 50);

        Text subText = RuntimeUi.CreateText(detailRoot.transform,
            $"{entry.category}   |   {d.archetype}   |   Maps: {maps}", 18, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        RectTransform srt = subText.rectTransform;
        srt.pivot = new Vector2(0.5f, 0.5f);
        srt.anchoredPosition = new Vector2(0, 105);
        srt.sizeDelta = new Vector2(900, 30);

        Text statsText = RuntimeUi.CreateText(detailRoot.transform,
            $"HP {d.maxHealth}   |   ATK {d.attackDamage}   |   BLOCK {d.block}", 22, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        RectTransform trt = statsText.rectTransform;
        trt.pivot = new Vector2(0.5f, 0.5f);
        trt.anchoredPosition = new Vector2(0, 60);
        trt.sizeDelta = new Vector2(900, 35);

        if (!string.IsNullOrEmpty(d.role))
        {
            Text roleLabel = RuntimeUi.CreateText(detailRoot.transform, "ROLE", 16, TextAnchor.UpperCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransform rlt = roleLabel.rectTransform;
            rlt.pivot = new Vector2(0.5f, 1f);
            rlt.anchoredPosition = new Vector2(0, 18);
            rlt.sizeDelta = new Vector2(900, 22);
            roleLabel.color = new Color(1f, 0.8f, 0.35f, 1f);
            roleLabel.horizontalOverflow = HorizontalWrapMode.Overflow;

            Text roleText = RuntimeUi.CreateText(detailRoot.transform, d.role, 20, TextAnchor.UpperCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransform rrt = roleText.rectTransform;
            rrt.pivot = new Vector2(0.5f, 1f);
            rrt.anchoredPosition = new Vector2(0, -6);
            rrt.sizeDelta = new Vector2(880, 150);
            roleText.color = new Color(0.95f, 0.95f, 1f, 1f);
            roleText.lineSpacing = 1.15f;
        }

        List<string> specials = GetSpecials(d);
        if (specials.Count > 0)
        {
            Text spText = RuntimeUi.CreateText(detailRoot.transform, string.Join("   |   ", specials), 18,
                TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransform sprt = spText.rectTransform;
            sprt.pivot = new Vector2(0.5f, 0.5f);
            sprt.anchoredPosition = new Vector2(0, -180);
            sprt.sizeDelta = new Vector2(900, 30);
        }

        if (entry.category != MonsterCategory.Normal)
        {
            Text mechText = RuntimeUi.CreateText(detailRoot.transform, BuildMechanicsText(entry), 15,
                TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransform mrt = mechText.rectTransform;
            mrt.pivot = new Vector2(0.5f, 0.5f);
            mrt.anchoredPosition = new Vector2(0, -260);
            mrt.sizeDelta = new Vector2(900, 380);
        }

        Text noteText = RuntimeUi.CreateText(detailRoot.transform,
            "*Base stats shown; in battle each map scales HP x(1+0.2xmap), dmg x(1+0.05xmap).", 13,
            TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        RectTransform nrt2 = noteText.rectTransform;
        nrt2.pivot = new Vector2(0.5f, 0.5f);
        nrt2.anchoredPosition = new Vector2(0, -480);
        nrt2.sizeDelta = new Vector2(900, 40);

        CreateDetailButton("Back to List", new Vector2(0, -520), new Vector2(220, 50), BackToList);
        CreateDetailButton("Close", new Vector2(230, -520), new Vector2(160, 50), TogglePanel);
    }

    private void BackToList()
    {
        detailRoot.SetActive(false);
        listRoot.SetActive(true);
    }

    private void CreateDetailButton(string label, Vector2 pos, Vector2 size, System.Action onClick)
    {
        GameObject go = new GameObject("DetailBtn_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(detailRoot.transform, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => onClick());

        RuntimeUi.CreateText(rt, label, 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
    }

    private static List<string> GetSpecials(EnemyData d)
    {
        List<string> specials = new();
        if (d.poisonDamage > 0) specials.Add($"Poison {d.poisonDamage}");
        if (d.lifesteal > 0) specials.Add($"Lifesteal {d.lifesteal}");
        if (d.selfHeal > 0) specials.Add($"Heal {d.selfHeal}");
        if (d.regenValue > 0) specials.Add($"Regen {d.regenValue}");
        if (d.buffStrength > 0) specials.Add($"Buff STR {d.buffStrength}");
        if (d.weakDamage > 0) specials.Add($"Weak {d.weakDamage}");
        if (d.vulnerableDamage > 0) specials.Add($"Vulnerable {d.vulnerableDamage}");
        if (d.counterStacks > 0) specials.Add($"Counter {d.counterStacks}");
        if (d.goldReward > 0) specials.Add($"Gold {d.goldReward}");
        return specials;
    }

    private static string BuildMechanicsText(MonsterCatalogEntry entry)
    {
        EnemyData d = entry.data;
        List<string> lines = new();
        lines.Add("-- Boss Mechanics --");

        if (d.phaseThreshold > 0f)
        {
            List<string> phase = new();
            if (d.phaseStrength > 0) phase.Add($"+{d.phaseStrength} Strength");
            if (d.phaseRegen > 0) phase.Add($"+{d.phaseRegen} Regen");
            if (d.phaseImmortal > 0) phase.Add($"Immortal {d.phaseImmortal} turn");
            if (d.phaseHeal > 0) phase.Add($"Heal {d.phaseHeal}");
            if (d.phasePlayerDebuff > 0) phase.Add($"Weak + Vulnerable {d.phasePlayerDebuff} on player");
            if (phase.Count == 0) phase.Add("buffs");
            lines.Add($"Phase 2 at <= {Mathf.RoundToInt(d.phaseThreshold * 100f)}% HP: {string.Join(", ", phase)}");
        }

        if (d.enrageMultiplier > 1)
            lines.Add($"Enrage: next attack x{d.enrageMultiplier}");

        if (d.canSummon && !string.IsNullOrEmpty(d.summonId))
            lines.Add($"Summon: {d.summonCount} x {d.summonId} at <= {Mathf.RoundToInt(d.summonThreshold * 100f)}% HP");

        lines.Add("Immune: Stun & instant-execute");
        lines.Add("");
        lines.Add("Tip: phase buffs trigger the moment HP crosses the threshold,");
        lines.Add("and the enraged attack is telegraphed one turn ahead");
        lines.Add("(a big number on the intent) - stack block before it lands.");

        return string.Join("\n", lines);
    }

    private Color GetFrameColor(MonsterCategory category)
    {
        switch (category)
        {
            case MonsterCategory.MiniBoss: return MiniFrame;
            case MonsterCategory.Boss: return BossFrame;
            default: return NormalFrame;
        }
    }

    private Sprite GetPlaceholder()
    {
        if (placeholderSprite == null)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            placeholderSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            placeholderSprite.name = "MonsterArtPlaceholder";
        }

        return placeholderSprite;
    }
}
