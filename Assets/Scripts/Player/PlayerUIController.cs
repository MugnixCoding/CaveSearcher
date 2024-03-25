using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerHealthState))]
public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private Image overlay;
    [SerializeField]
    private float duration = 2f;
    [SerializeField]
    private float fadeSpeed = 2f;
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private Image compass;
    [SerializeField]
    private TextMeshProUGUI hintText;
    [SerializeField]
    private float hintDuration = 5f;
    [SerializeField]
    private Image interactHint;

    private float damageDurationTimer;
    private bool isDamageCoroutineExecuting;
    private float hintDurationTimer;
    private bool isHintCoroutineExecuting;
    private Vector3 direction;
    private void Start()
    {
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        isDamageCoroutineExecuting = false;
        isHintCoroutineExecuting = false;
        PlayerHealthState playerHealthState = gameObject.GetComponent<PlayerHealthState>();
        playerHealthState.OnTakeDamage += TakeDamage;
        playerHealthState.OnDead += Death;
        healthText.SetText(gameObject.GetComponent<PlayerHealthState>().GetPlayerHealth().ToString());
        hintText.SetText("");

        Interactor interactor = gameObject.GetComponent<Interactor>();
        interactor.OnDetectInterectable += ShowHintButton;
        interactor.OnNoInterectable += HideHintButton;
    }
    private void Update()
    {
        direction.z = transform.eulerAngles.y;
        compass.transform.localEulerAngles = direction;
    }
    #region health hurt dead
    private void TakeDamage(object sender,DamageEventArgs e)
    {
        damageDurationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.5f);
        if (!isDamageCoroutineExecuting)
        {
            StartCoroutine(DamageOverlayFade());
        }
        healthText.SetText(gameObject.GetComponent<PlayerHealthState>().GetPlayerHealth().ToString());
    }
    private void Death(object sender, EventArgs e)
    {
        damageDurationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.5f);
        healthText.SetText(gameObject.GetComponent<PlayerHealthState>().GetPlayerHealth().ToString());
    }
    private IEnumerator DamageOverlayFade()
    {
        isDamageCoroutineExecuting = true;
        while (overlay.color.a > 0)
        {
            damageDurationTimer += Time.deltaTime;
            if (damageDurationTimer >= duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
            yield return new WaitForFixedUpdate();
        }
        isDamageCoroutineExecuting = false;
    }
    #endregion

    #region talk message
    public void TalkHintMessage(string message)
    {
        hintDurationTimer = 0;
        hintText.SetText(message);
        if (!isHintCoroutineExecuting)
        {
            StartCoroutine(TalkHintFade());
        }
    }
    private IEnumerator TalkHintFade()
    {
        isHintCoroutineExecuting = true;
        while (hintDurationTimer < hintDuration)
        {
            hintDurationTimer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        hintText.SetText("");
        isHintCoroutineExecuting = false; ;
    }
    #endregion
    #region hint
    private void ShowHintButton(object sender, EventArgs e)
    {
        interactHint.gameObject.SetActive(true);
    }
    private void HideHintButton(object sender, EventArgs e)
    {
        interactHint.gameObject.SetActive(false);
    }
    #endregion

}
