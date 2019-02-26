/*============================================================================== 
Copyright (c) 2017 PTC Inc. All Rights Reserved.

 * Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;

/// <summary>
/// Class to parse the JSON Data after a Request.
/// </summary>
public class JSONParser
{
    #region CONSTANTS
    private const string TITLE_KEY = "title";
    private const string AUTHOR_KEY = "author";
    private const string AVERAGE_RATING_KEY = "average rating";
    private const string NUMB_OF_RATINGS_KEY = "# of ratings";
    private const string LIST_PRICE_KEY = "list price";
    private const string YOUR_PRICE_KEY = "your price";
    private const string THUMB_URL_KEY = "thumburl";
    private const string BOOK_URL_KEY = "bookurl";
    #endregion //CONSTANTS


    #region PUBLIC_METHODS
    /// <summary>
    /// Parses a JSON string and returns a book data struct from that
    /// </summary>
    public BookData ParseString(string val)
    {
        BookData bookData = new BookData();
        string[] jsonLines = val.Split(',');
        
        foreach (string line in jsonLines)
        {
            string[] elements =  line.Split(':');
            if (elements.Length > 1)
            {
                string jsonKey = GetJSONValue(elements[0]);
                
                if (jsonKey.Equals(TITLE_KEY))
                {
                    bookData.SetBookTitle(GetJSONValue(elements[1]));
                    
                }
                else if (jsonKey.Equals(AUTHOR_KEY))
                {
                    bookData.SetBookAuthor(GetJSONValue(elements[1]));
                    
                }
                else if (jsonKey.Equals(AVERAGE_RATING_KEY))
                {
                    bookData.SetBookRating(int.Parse(GetJSONValue(elements[1])));
                    
                }
                else if (jsonKey.Equals(NUMB_OF_RATINGS_KEY))
                {
                    bookData.SetBookOverallRating(int.Parse(GetJSONValue(elements[1])));
                    
                }
                else if (jsonKey.Equals(LIST_PRICE_KEY))
                {
                    bookData.SetBookRegularPrice(float.Parse(GetJSONValue(elements[1])));
                    
                }
                else if (jsonKey.Equals(YOUR_PRICE_KEY))
                {
                    bookData.SetBookYourPrice(float.Parse(GetJSONValue(elements[1])));
                    
                }
                else if (jsonKey.Equals(THUMB_URL_KEY))
                {
                    string thumbUrl = "";
                    for(int j=1; j < elements.Length; j++)
                    {
                        thumbUrl += elements[j];
                        if(j == 1)
                        {
                            thumbUrl += ":";    
                        }
                    }
                    
                    bookData.SetBookThumbUrl(GetJSONValue(thumbUrl));
                    
                }
                else if (jsonKey.Equals(BOOK_URL_KEY))
                {
                    string bookUrl = "";
                    for(int j=1; j< elements.Length; j++)
                    {
                        bookUrl += elements[j];    
                        if(j == 1)
                        {
                            bookUrl += ":";    
                        }
                    }

                    bookData.SetBookDetailUrl(GetJSONValue(bookUrl));
                }
            }
        }
        
        return bookData;
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private string GetJSONValue(string val)
    {
        int initialIndex = val.IndexOf('"');
        int finalIndex = val.LastIndexOf('"');
        int lenght = finalIndex - (initialIndex+1);
        return val.Substring(initialIndex+1,lenght);
    }
    #endregion //PRIVATE_METHODS
}
