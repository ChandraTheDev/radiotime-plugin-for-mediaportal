﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;

namespace RadioTimePlugin
{
  public static class Translation
  {
    #region Private variables

    //private static Logger logger = LogManager.GetCurrentClassLogger();
    private static Dictionary<string, string> _translations;
    private static readonly string _path = string.Empty;
    private static readonly DateTimeFormatInfo _info;

    #endregion

    #region Constructor

    static Translation()
    {
      string lang;

      try
      {
        lang = GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage());
        _info = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentUICulture);
      }
      catch (Exception)
      {
        lang = CultureInfo.CurrentUICulture.Name;
        _info = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentUICulture);
      }

      Log.Info("Using language " + lang);

      _path = Config.GetSubFolder(Config.Dir.Language, "RadioTime");

      if (!System.IO.Directory.Exists(_path))
        System.IO.Directory.CreateDirectory(_path);

      LoadTranslations(lang);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the translated strings collection in the active language
    /// </summary>
    public static Dictionary<string, string> Strings
    {
      get
      {
        if (_translations == null)
        {
          _translations = new Dictionary<string, string>();
          Type transType = typeof(Translation);
          FieldInfo[] fields = transType.GetFields(BindingFlags.Public | BindingFlags.Static);
          foreach (FieldInfo field in fields)
          {
            _translations.Add(field.Name, field.GetValue(transType).ToString());
          }
        }
        return _translations;
      }
    }

    #endregion

    #region Public Methods

    public static int LoadTranslations(string lang)
    {
      XmlDocument doc = new XmlDocument();
      Dictionary<string, string> TranslatedStrings = new Dictionary<string, string>();
      string langPath = "";
      try
      {
        langPath = Path.Combine(_path, lang + ".xml");
        doc.Load(langPath);
      }
      catch (Exception e)
      {
        if (lang == "en")
          return 0; // otherwise we are in an endless loop!

        if (e.GetType() == typeof(FileNotFoundException))
          Log.Warn("Cannot find translation file {0}.  Failing back to English", langPath);
        else
        {
          Log.Error("Error in translation xml file: {0}. Failing back to English", lang);
          Log.Error(e);
        }

        return LoadTranslations("en");
      }
      foreach (XmlNode stringEntry in doc.DocumentElement.ChildNodes)
      {
        if (stringEntry.NodeType == XmlNodeType.Element)
          try
          {
            TranslatedStrings.Add(stringEntry.Attributes.GetNamedItem("Field").Value, stringEntry.InnerText);
          }
          catch (Exception ex)
          {
            Log.Error("Error in Translation Engine");
            Log.Error(ex);
          }
      }

      Type TransType = typeof(Translation);
      FieldInfo[] fieldInfos = TransType.GetFields(BindingFlags.Public | BindingFlags.Static);
      foreach (FieldInfo fi in fieldInfos)
      {
        if (TranslatedStrings != null && TranslatedStrings.ContainsKey(fi.Name))
          TransType.InvokeMember(fi.Name, BindingFlags.SetField, null, TransType, new object[] { TranslatedStrings[fi.Name] });
        else
          Log.Info("Translation not found for field: {0}.  Using hard-coded English default.", fi.Name);
      }
      return TranslatedStrings.Count;
    }

    public static string GetByName(string name)
    {
      if (!Strings.ContainsKey(name))
        return name;

      return Strings[name];
    }

    public static string GetByName(string name, params object[] args)
    {
      return String.Format(GetByName(name), args);
    }

    /// <summary>
    /// Takes an input string and replaces all ${named} variables with the proper translation if available
    /// </summary>
    /// <param name="input">a string containing ${named} variables that represent the translation keys</param>
    /// <returns>translated input string</returns>
    public static string ParseString(string input)
    {
      Regex replacements = new Regex(@"\$\{([^\}]+)\}");
      MatchCollection matches = replacements.Matches(input);
      foreach (Match match in matches)
      {
        input = input.Replace(match.Value, GetByName(match.Groups[1].Value));
      }
      return input;
    }


    //public static string GetMediaType(MediaType mediaType)
    //{
    //  switch (mediaType)
    //  {
    //    case MyAlarm.MediaType.File:
    //      return File;

    //    case MyAlarm.MediaType.PlayList:
    //      return Playlist;

    //    case MyAlarm.MediaType.Message:
    //      return Message;

    //    default:
    //      return String.Empty;
    //  }
    //}

    public static string GetDayName(DayOfWeek dayOfWeek)
    {
      return _info.GetDayName(dayOfWeek);
    }
    public static string GetShortestDayName(DayOfWeek dayOfWeek)
    {
      return _info.GetShortestDayName(dayOfWeek);
    }

    #endregion

    #region Translations / Strings

    /// <summary>
    /// These will be loaded with the language files content
    /// if the selected lang file is not found, it will first try to load en(us).xml as a backup
    /// if that also fails it will use the hardcoded strings as a last resort.
    /// </summary>
    

    // A
    public static string AddToFavorites = "Add to favorites";

    // C
    public static string ComunicationError = "Comunication error or wrong user name or password";
    
    // D


    // E
    public static string PlayError = "Error in  playback {0}";
    public static string Empty = "Empty";

    // F
    public static string FastPresets = "Fast presets";
    public static string Folders = "Folders";

    // G
    public static string Genres = "Genres";

    // H
    public static string Home = "Home";

    // I
    public static string Items = "Items";

    // L

    // M
    public static string Message = "Message";

    // N
    public static string NewSearch = "New Search";
    public static string NoSorting = "No sorting";
    public static string NowPlaying = "Now Playing";
    public static string NoStationsOrShowsAvailable = "No stations or shows available";
    public static string NoPresetFoldersFound = "No preset folders found";

    // O
    public static string Objects = "Objects";

    // P
    public static string Presets = "My Presets";

    // R
    public static string RemoveFromFavorites = "Remove from favorites";
    public static string Random = "Random Play";

    // S
    public static string Search = "Search";
    public static string SearchHistory = "Search History";
    public static string SearchArtist = "Search Artist";
    public static string SelectPresetFolder = "Select preset folder";
    public static string SelectPresetNumber = "Select preset number";
    public static string SortByBitrate = "Sort by bitrate";
    public static string SortByName = "Sort by name";
    public static string Sorting = "Sorting";
    public static string ShowGiuide = "Show guide";
    public static string StationNotAvaiable = "The station isn't avaiable";
    public static string SimilarStations = "Similar stations";    

    // T


    // U
    public static string UseWebInterfaceToEditFavorites = "Use web interface to edit favorites";
    
    // V

    // W

    // Y

    #endregion

  }

}