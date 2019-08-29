﻿using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AddJobList : MonoBehaviour {
    public static AddJobList Instance;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    //public GameObject DepGrid;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    const int pageSize = 10;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pegeTotalText;
    /// <summary>
    /// 输入页数
    /// </summary>
    public InputField pegeNumText;
    /// <summary>
    /// 筛选后的数据
    /// </summary>
    List<Post> ScreenList;
    /// <summary>
    /// 部门总数据
    /// </summary>
    List<Post> JobData;
    /// <summary>
    /// 展示的10条信息
    /// </summary>
    List<Post> ShowList;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;

    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;
    
    public Button Addpst;
    public InputField JobSelected;
    public Button selectedBut;
    public GameObject JobListUI;
    public Button CloseBut;
    public Sprite DoubleImage;
    public Sprite OddImage;
    void Start()
    {
        Instance = this;
        ShowList = new List<Post>();
        AddPageBut.onClick.AddListener(AddJobPage);
        MinusPageBut.onClick.AddListener(MinusJobPage);
        pegeNumText.onValueChanged.AddListener(InputJobPage);
        selectedBut.onClick.AddListener(SetJob_Click);
        CloseBut.onClick.AddListener(()=>
        {
            UGUIMessageBox.Show("关闭部门信息！",
    () =>
    {
        CloseJobListWindow();
        AddPersonnel.Instance.ShowAddPerWindow();
    }, ()=>
    {
        CloseJobListWindow();
        AddPersonnel.Instance.ShowAddPerWindow();
    });
          
        });
       
        Addpst.onClick.AddListener(() =>
       {
           AddJobs.Instance.isAdd = false;
           CloseJobListWindow();
           AddJobs.Instance . ShowJobEditWindow();
           AddJobs.Instance.GetPostList(JobData);
       });
    }

    public void GetJobListData(List<Post> info)
    {
        SaveSelection();
        ScreenList = new List<Post>();
        JobData = new List<Post>();
        if (ScreenList .Count != 0)
        {
            ScreenList.Clear();
        }
        if (JobData .Count != 0)
        {
            JobData.Clear();
        }
        ScreenList.AddRange(info);
        JobData.AddRange(info);
        TotaiLine(info);
        pegeNumText.text = "1";
        GetPageData(JobData);
    }
    void Update()
    {

    }
    public void SetJobData(List<Post> info)
    {
        for (int i = 0; i < info.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            AddJobItem item = Obj.GetComponent<AddJobItem>();
            item.ShowJobItemInfo(info[i]);
            if (i % 2 == 0)
            {
                item.GetComponent<Image>().sprite = DoubleImage;
            }
            else
            {
                item.GetComponent<Image>().sprite = OddImage;
            }
        }
    }
    public void GetPageData(List<Post> info)
    {
        SaveSelection();
        if (StartPageNum * pageSize < info.Count)
        {
            var QueryData = info.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                ShowList.Add(per);
            }
            SetJobData(ShowList);
        }
        ShowList.Clear();
    }
    public void AddJobPage()
    {
        StartPageNum += 1;
        if (StartPageNum <= ScreenList.Count / pageSize)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPageData(ScreenList);
        }
        else
        {
            StartPageNum -= 1;
        }
    }
    public void MinusJobPage()
    {
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pegeNumText.text = "1";
                GetPageData(ScreenList);
            }
            else
            {
                pegeNumText.text = PageNum.ToString();
            }
        }
    }
    public void InputJobPage(string value)
    {
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumText.text);
        }

        int maxPage = (int)Math.Ceiling((double)(ScreenList.Count) / (double)pageSize);
        if (currentPage > maxPage)
        {
            currentPage = maxPage;
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(ScreenList);
    }
    public void SetJob_Click()
    {
        StartPageNum = 0;
        PageNum = 1;
        pegeNumText.text = "1";
        ScreenList.Clear();
        SaveSelection();
        string key = JobSelected.text.ToString().ToLower();
        for (int i = 0; i < JobData.Count; i++)
        {
            string name = JobData[i].Name;
            if (name.ToLower().Contains(key))
            {
                ScreenList.Add(JobData[i]);
            }
        }
        if (ScreenList.Count == 0)
        {
            pegeTotalText.text = "1";
        }
        else
        {
            TotaiLine(ScreenList);
            GetPageData(ScreenList);
        }
    }
    public void ShowJobListWindow()
    {
        JobListUI.SetActive(true);

    }
    
    public void CloseJobListWindow()
    {
        JobListUI.SetActive(false);
    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    /// <summary>
    /// 有几页数据
    /// </summary>
    /// <param name="data"></param>
    public void TotaiLine(List<Post> dinfo)
    {
        if (dinfo.Count % pageSize == 0)
        {
            pegeTotalText.text = (dinfo.Count / pageSize).ToString();
        }
        else
        {
            pegeTotalText.text = Convert.ToString(Math.Ceiling((double)(dinfo.Count) / (double)pageSize));
        }
    }
    /// <summary>
    /// 保留选中项
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
   
}