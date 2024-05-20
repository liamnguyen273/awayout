using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EffectCapture : MonoBehaviour
{
    public ButtonWeapon buttonWeapon;
    public ButtonSoldier buttonSoldier;
    public int imageWidth = 1080;
    public int imageHeight = 1080;
    public SnapshotCamera snapshotCamera;
    public ItemDatabase itemDatabase;
    public Transform soldiers;
    void Start()
    {
        snapshotCamera = SnapshotCamera.MakeSnapshotCamera("Default");
        //snapshotCamera = SnapshotCamera.MakeSnapshotCamera("Board");
    }

    public void StartCapture()
    {
        StartCoroutine(RoutineCaptureCard());

    }

    private IEnumerator RoutineCaptureCard()
    {
        buttonSoldier.gameObject.SetActive(false);
        buttonWeapon.gameObject.SetActive(true);
        soldiers.gameObject.SetActive(false);
        for (int i = 0; i < itemDatabase.weaponDatas.Length; i++)
        {
            buttonWeapon.Init(itemDatabase.weaponDatas[i], "");
            yield return StartCoroutine(TakeSnapShot(itemDatabase.weaponDatas[i].itemId));
        }

        buttonSoldier.gameObject.SetActive(true);
        buttonWeapon.gameObject.SetActive(false);
        soldiers.gameObject.SetActive(false);
        for (int i = 0; i < itemDatabase.characterProperties.Length; i++)
        {
            buttonSoldier.Init(itemDatabase.characterProperties[i], "");
            yield return StartCoroutine(TakeSnapShot(itemDatabase.characterProperties[i].assetId));
        }


    }

    private IEnumerator RoutineCaptureSoldiers()
    {
        buttonSoldier.gameObject.SetActive(false);
        buttonWeapon.gameObject.SetActive(false);
        soldiers.gameObject.SetActive(true);

        foreach (Transform child in soldiers)
        {
            foreach (Transform child2 in soldiers)
            {
                child2.gameObject.SetActive(false);
            }
            child.gameObject.SetActive(true);
            WeaponData randomWeapon = itemDatabase.weaponDatas[Random.Range(0, itemDatabase.weaponDatas.Length)];
            CharacterModel model = child.GetComponent<CharacterModel>();
            ProjectileWeapon gun = PoolingManager.Instance.GetGameObject<ProjectileWeapon>(randomWeapon.weaponPrefab, model.rightHand);
            model.animator.SetInteger("GunIndex", gun.gunIndex);
            gun.transform.localPosition = Vector3.zero;
            gun.transform.localRotation = Quaternion.identity;
            gun.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(TakeSnapShot(child.name));
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Start");
            StartCapture();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Start");
            StartCoroutine(RoutineCaptureSoldiers());
        }
    }
    private IEnumerator TakeSnapShot(string fileName)
    {
        snapshotCamera.defaultPositionOffset = new Vector3(0, 0, 10);
        snapshotCamera.defaultRotation = Vector3.zero;
        Texture2D texture = snapshotCamera.TakeObjectSnapshot(buttonWeapon.gameObject, imageWidth, imageHeight);
        System.IO.FileInfo fi = SnapshotCamera.SavePNG(texture, fileName);
        Debug.Log((string.Format("Snapshot {0} saved to {1}", fi.Name, fi.DirectoryName)));
        Destroy(texture);
        yield return new WaitUntil(() => File.Exists(fi.FullName));

        yield return new WaitForEndOfFrame();


    }


}
