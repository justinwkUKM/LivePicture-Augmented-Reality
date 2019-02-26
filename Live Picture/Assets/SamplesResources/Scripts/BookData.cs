/*============================================================================== 
Copyright (c) 2017 PTC Inc. All Rights Reserved.

 * Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

/// <summary>
/// Model containing the book information, used by the BookInformationParser class.
/// </summary>
public class BookData
{
    #region PROPERTIES
    public string BookTitle
    {
        get; private set;
    }

    public string BookAuthor
    {
        get; private set;
    }

    public int BookRating
    {
        get; private set;
    }

    public int BookOverallRating
    {
        get; private set;
    }

    public float BookRegularPrice
    {
        get; private set;
    }

    public float BookYourPrice
    {
        get; private set;
    }

    public string BookThumbUrl
    {
        get; private set;
    }

    public string BookDetailUrl
    {
        get; private set;
    }
    #endregion // PROPERTIES


    #region PUBLIC_METHODS
    public void SetBookTitle(string title)
    {
        int wrapIndex = 0;
        if(title.Length > 18 )
        {
            char[] characters = title.ToCharArray();
            for(int i = 18; i > 0; i --){
            
                if(characters[i].Equals(' '))
                {
                    wrapIndex = i;
                    i =0;
                }
            }
            title = title.Insert(wrapIndex +1,"\n");
        }
        BookTitle = title;
    }
    
    public void SetBookAuthor(string author)
    {
        BookAuthor = author;
    }
    
    public void SetBookRating(int rating)
    {
        BookRating = rating;
    }
    
    public void SetBookOverallRating(int rating )
    {
        BookOverallRating = rating;
    }
    
    public void SetBookRegularPrice(float price )
    {
        BookRegularPrice = price;
    }
    
    public void SetBookYourPrice(float price )
    {
        BookYourPrice = price;    
    }
    
    public void SetBookThumbUrl(string url)
    {
        url = url.Replace('"',' ');
        url = url.Trim();
        BookThumbUrl = url;    
    }
    
    public void SetBookDetailUrl(string url)
    {
        url = url.Replace('"',' ');
        url = url.Trim();
        BookDetailUrl = url;
    }
    #endregion // PUBLIC_METHODS
}
