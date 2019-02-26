/*============================================================================== 
Copyright (c) 2017 PTC Inc. All Rights Reserved.

 * Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;

/// <summary>
/// This class manages the parsing of Book Data.
/// </summary>
public class BookInformationParser
{
    #region PRIVATE_MEMBERS
    private GameObject mBookObject;
    private TextMesh mBookTitle;
    private TextMesh mBookAuthor;
    private TextMesh mBookRegularPrice;
    private TextMesh mBookOverallRatings;
    private StarsRatingControl mBookStarsRating;
    private TextMesh mBookYourPrice;
    private GameObject mBookThumb;
    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_METHODS
    /// <summary>
    /// Updates the book data in the bookObject gameObject
    /// </summary>
    public void UpdateBookData(BookData bookData)
    {
        if (mBookObject != null )
        {
            mBookTitle.text = bookData.BookTitle;
            mBookAuthor.text = bookData.BookAuthor;
            mBookRegularPrice.text = string.Format("${0}", bookData.BookRegularPrice);
            mBookOverallRatings.text = string.Format("( {0} ratings )", bookData.BookOverallRating);
            mBookYourPrice.text =  string.Format("${0}", bookData.BookYourPrice);
            mBookStarsRating.SetRating(bookData.BookRating);
        }
    }
    
    /// <summary>
    /// Sets the book object and initialize all the book data gameobjects
    /// </summary>
    public void SetBookObject(GameObject bookObject)
    {
        mBookObject = bookObject;

        Transform bookTransform = bookObject.transform;
        mBookTitle = bookTransform.Find("BookData/BookTitle").GetComponent<TextMesh>();
        mBookAuthor = bookTransform.Find("BookData/BookAuthor").GetComponent<TextMesh>();
        mBookRegularPrice = bookTransform.Find("BookData/RegularPriceObjects/RegularPrice").GetComponent<TextMesh>();
        mBookOverallRatings = bookTransform.Find("BookData/RatingObjects/RatingsText").GetComponent<TextMesh>();
        mBookYourPrice = bookTransform.Find("BadgeObjects/BadgeBackground/DiscountPrice").GetComponent<TextMesh>();
        mBookStarsRating = bookTransform.Find("BookData/RatingObjects/StarsContainer").GetComponent<StarsRatingControl>();
        mBookThumb = bookTransform.Find("BookThumb").gameObject;
        
        GameObject touchForMoreInfoObject = mBookObject.transform.Find("TouchForMoreInformation").gameObject;
        
        if (touchForMoreInfoObject != null )
        {
            touchForMoreInfoObject.GetComponent<TextMesh>().GetComponent<Renderer>().material.SetColor("_Color", new Color(0.2f, 0.7098f, 0.898f, 1));
        }
    }
    
    /// <summary>
    /// Updates the book thumbnail image
    /// </summary>
    public void UpdateBookThumb(Texture bookThumbTexture)
    {
        if (mBookThumb != null )
        {
            mBookThumb.GetComponent<Renderer>().material.mainTexture = bookThumbTexture;
        }
    }
    #endregion // PUBLIC_METHODS
}
