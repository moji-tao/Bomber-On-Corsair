using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private PlayerControl player;
    private Door doorExit;

    public bool gameOver;

    public List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
        //player = FindObjectOfType<PlayerControl>();
        //doorExit = FindObjectOfType<Door>();
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            gameOver = player.isDead;
            UIManager.instance.GameOverUI(gameOver);
        }
    }

    public void IsEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void IsPlayer(PlayerControl controller)
    {
        player = controller;
    }

    public void IsExitDoor(Door door)
    {
        doorExit = door;
    }

    public void EnemyDead(Enemy enemy)//怪物全死亡开门
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            if (enemies.Count == 0 && doorExit != null)
            {
                //非BOSS关开门
                doorExit.OpenDoor();
                Debug.Log("1");
            }
            else if (enemies.Count == 0 && doorExit == null)
            {
                //BOSS关显示WIN
                Debug.Log("2");
            }
        }
        else
            return;
    }

    public void NewGame()//新游戏
    {
        PlayerPrefs.DeleteAll();
        //SaveData();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()//继续游戏
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("sceneIndex"));
    }

    public void Replay()//重启当前场景
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()//进入下一关
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retitle()//返回标题
    {
        SceneManager.LoadScene(0);

        Time.timeScale = 1;
    }

    public void Quit()//退出游戏
    {
        Application.Quit();
    }

    public float LoadHealth()//返回血量
    {
        if(!PlayerPrefs.HasKey("playerHealth"))
        {
            PlayerPrefs.SetFloat("playerHealth",player.health);
        }

        return PlayerPrefs.GetFloat("playerHealth");
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("sceneIndex", SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetFloat("playerHealth", player.health);
        PlayerPrefs.Save();
    }
}
