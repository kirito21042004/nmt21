using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider volumeSlider;
    public Slider fbxSlider;
    public Slider lightSlider;
    public Toggle muteToggle;

    // THÊM: đèn cần điều chỉnh độ sáng
    public Light sceneLight;

    // THÊM: các AudioSource chứa âm thanh hiệu ứng
    public AudioSource[] fbxAudioSources;

    void Start()
    {
        // THÊM: tạo giá trị mặc định trong lần chạy đầu tiên
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1f);
        }

        if (!PlayerPrefs.HasKey("fbx"))
        {
            PlayerPrefs.SetFloat("fbx", 1f);
        }

        if (!PlayerPrefs.HasKey("light"))
        {
            PlayerPrefs.SetFloat("light", 1f);
        }

        if (!PlayerPrefs.HasKey("mute"))
        {
            PlayerPrefs.SetInt("mute", 0);
        }

        // khi mở màn hình thì lấy dữ liệu cũ , cập nhập vào slider
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        fbxSlider.value = PlayerPrefs.GetFloat("fbx");
        lightSlider.value = PlayerPrefs.GetFloat("light");
        muteToggle.isOn = PlayerPrefs.GetInt("mute") == 1 ? true : false;

        // THÊM: áp dụng dữ liệu đã lưu
        ChangeVolume(volumeSlider.value);
        ChangeFbx(fbxSlider.value);
        ChangeLight(lightSlider.value);
        ChangeMute(muteToggle.isOn);
    }

    public void Save()
    {
        float volume = volumeSlider.value;
        float fbx = fbxSlider.value;
        float light = lightSlider.value;
        bool mute = muteToggle.isOn ? true : false;

        // Class PlayerPrefs cho phép lưu biến vào bộ nhớ , gọi ra ở các lần sau
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("fbx", fbx);
        PlayerPrefs.SetFloat("light", light);
        PlayerPrefs.SetInt("mute", mute ? 1 : 0);
        PlayerPrefs.Save();

        // THÊM: áp dụng ngay sau khi lưu
        ChangeVolume(volume);
        ChangeFbx(fbx);
        ChangeLight(light);
        ChangeMute(mute);

        // Hiển thị thông báo đã lưu thành công
        Debug.Log("Đã lưu Setting thành công");
    }

    // THÊM: điều chỉnh âm lượng tổng
    public void ChangeVolume(float value)
    {
        if (muteToggle.isOn)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = value;
        }
    }

    // THÊM: điều chỉnh âm lượng hiệu ứng
    public void ChangeFbx(float value)
    {
        foreach (AudioSource audioSource in fbxAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = value;
            }
        }
    }

    // THÊM: điều chỉnh độ sáng
    public void ChangeLight(float value)
    {
        if (sceneLight != null)
        {
            sceneLight.intensity = value;
        }

        RenderSettings.ambientIntensity = value;
    }

    // THÊM: bật hoặc tắt toàn bộ âm thanh
    public void ChangeMute(bool mute)
    {
        if (mute)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = volumeSlider.value;
        }
    }

    public void Back()
    {
        // Quay tro lai MenuScene
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {

    }
}