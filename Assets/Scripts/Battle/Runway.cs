using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Runway : MonoBehaviour
{

    float len = 100;

    private Queue<CreatureBase> burstWaitingQueue;
    private List<RunwayAvatar> burstAvatars;
    private List<RunwayAvatar> runwayAvatars;
    Dictionary<CreatureBase, RunwayAvatar> creature2RunwayAvatar;
    private List<CreatureBase> creatures = new List<CreatureBase>();


    public GameObject[] burstAvatarGO;
    public RectTransform[] burstAvatarRect;
    public Image[] burstAvartarImage;

    public GameObject avatarPrefab;
    public Transform runwayTransform;


    private readonly Vector3 firstRunwayAvatarPos = new Vector3(10, -25, 0);
    private readonly Vector3 runwayAvatarInternal = new Vector3(0, -55, 0);

    void Start()
    {
        burstWaitingQueue = new Queue<CreatureBase>();
        burstAvatars = new List<RunwayAvatar>();
        runwayAvatars = new List<RunwayAvatar>();
        creature2RunwayAvatar = new Dictionary<CreatureBase, RunwayAvatar>();
    }

    private bool firstTime = true;

    public void AddCreature(CreatureBase c)
    {
        creatures.Add(c);
        c.ChangeLocation(-100);
        RunwayAvatar newOne = Instantiate(avatarPrefab, runwayTransform).GetComponent<RunwayAvatar>();
        newOne.SetCreature(c, false);
        newOne.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(10, -500, 0);
        runwayAvatars.Add(newOne);
        creature2RunwayAvatar[c] = newOne;
    }

    public CreatureBase UpdateRunway(out bool isBurst)
    {
        isBurst = burstWaitingQueue.Count > 0;
        if (!firstTime)
        {
            // 移除第一个 avatar，如果被移除的不是 burst，在队尾创建一个新的
            RunwayAvatar firstAvatar = runwayAvatars[0];
            firstAvatar.MoveTowards(new Vector3(10, 25, 0), () => { Destroy(firstAvatar.gameObject); });
            runwayAvatars.RemoveAt(0);
            if (!firstAvatar.IsBurst)
            {
                RunwayAvatar newOne = Instantiate(avatarPrefab, runwayTransform).GetComponent<RunwayAvatar>();
                newOne.SetCreature(firstAvatar.creature, false);
                newOne.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(10, -500, 0);
                runwayAvatars.Add(newOne);
                creature2RunwayAvatar[firstAvatar.creature] = newOne;
            }
        }
        firstTime = false;

        // 有插入的大招时，先放大招
        if (isBurst)
        {
            for(int i  =0; i < burstAvatars.Count; ++i)
            {
                burstAvatars[i].MoveTowards(firstBurstEndPos + i * burstAvatarInternal);// burstEndPos[i]);
            }
            runwayAvatars.Insert(0, burstAvatars[0]);
            burstAvatars.RemoveAt(0);
            return burstWaitingQueue.Dequeue();
        }

        // 非大招回合，update 所有人的进度，更新UI
        float fastest_time = 100;
        for(int i = 0; i < creatures.Count; ++i)
        {
            fastest_time = Mathf.Min((len - creatures[i].location) / creatures[i].GetFinalAttr(null, null, CommonAttribute.Speed), fastest_time);
        }
        foreach(CreatureBase c in creatures)
        {
            c.ChangeLocation(c.location + fastest_time * c.GetFinalAttr(null, null, CommonAttribute.Speed));
        }
        creatures.Sort((c1, c2) =>
        {
            if (c1.location == c2.location) 
                return 0;
            return c1.location < c2.location ? 1 : -1;
        });
        RearrangeRunwayUI();
        return creatures[0];
    }

    private void RearrangeRunwayUI()
    {
        runwayAvatars.Sort((ra1, ra2) =>
       {
           if (ra1.IsBurst) return 1;
           if (ra2.IsBurst) return -1;
           if (ra1.creature.location == ra2.creature.location) return 0;
           return ra1.creature.location < ra2.creature.location ? 1 : -1;
       });
        for(int i = 0; i<runwayAvatars.Count; ++i)
        {
            runwayAvatars[i].MoveTowards(firstRunwayAvatarPos + i * runwayAvatarInternal);
        }
    }

    public void RemoveCreature(CreatureBase creature)
    {
        creatures.Remove(creature);
        RunwayAvatar ra = runwayAvatars.Find(r =>
        {
            return r.creature == creature;
        });
        Destroy(ra.gameObject);
        runwayAvatars.Remove(ra);
    }

    public void InsertBurst(CreatureBase c, bool immediately = false)
    {
        // 首先创建一个新的 avatar，设置它的 creature
        GameObject go = Instantiate(avatarPrefab, runwayTransform);
        go.GetComponent<RectTransform>().anchoredPosition = firstBurstStartPos + burstWaitingQueue.Count * burstAvatarInternal;
        go.GetComponent<Image>().sprite = c.mono.runwayAvatar;
        RunwayAvatar ra = go.GetComponent<RunwayAvatar>();
        ra.SetCreature(c, true);

        if (!immediately)
        { // 如果现在是 burst / 敌人的回合，那么当前插入的回合去等待队列
            burstWaitingQueue.Enqueue(c);
            burstAvatars.Add(ra);
            ra.MoveTowards(firstBurstEndPos + burstWaitingQueue.Count * burstAvatarInternal);
        }
        else
        { // 如果现在不是 burst，那么当前插入的回合到 runway 头，其他人后退
            for (int i = 0; i < runwayAvatars.Count; ++i)
            {
                runwayAvatars[i].MoveTowards(firstRunwayAvatarPos + (i + 1) * runwayAvatarInternal);
            }
            runwayAvatars.Insert(0, ra);
            ra.MoveTowards(firstRunwayAvatarPos);
        }
    }

    private readonly Vector3 firstBurstStartPos = new Vector3(200, -25, 0);
    private readonly Vector3 firstBurstEndPos = new Vector3(10, -25, -0);
    private readonly Vector3 burstAvatarInternal = new Vector3(70, 0, 0);
}
