using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteManager : MonoBehaviour {

	public Canvas parentCanvas;

	public GameObject quoteBoxObj;

	public void DisplayQuote(AthleteController speaker, string quote) {
        GameObject newQuote = Instantiate(quoteBoxObj, Vector3.zero, Quaternion.identity, parentCanvas.transform);
        QuoteBox quoteBox = newQuote.GetComponent<QuoteBox>();

        quoteBox.SetQuoteBox(speaker, quote);
    }
}
