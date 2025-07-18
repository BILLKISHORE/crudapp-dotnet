class AppConstants {
  // Suggested questions for the home screen
  static const List<String> suggestedQuestions = [
    "What are the latest developments in AI?",
    "How does climate change affect global weather patterns?",
    "What are the benefits of renewable energy?",
    "Explain quantum computing in simple terms",
    "What is the current state of space exploration?",
    "How do cryptocurrencies work?",
    "What are the health benefits of meditation?",
    "Explain the basics of machine learning",
    "What are the latest scientific discoveries?",
    "How does the human brain process information?",
  ];

  // App strings
  static const String appName = 'SageSearch';
  static const String searchHint = 'Ask anything...';
  static const String emptyHistoryMessage = 'No search history yet';
  static const String sourcesTitle = 'Sources';
  static const String historyTitle = 'Search History';
  
  // Animation durations
  static const Duration shortAnimation = Duration(milliseconds: 200);
  static const Duration mediumAnimation = Duration(milliseconds: 300);
  static const Duration longAnimation = Duration(milliseconds: 500);
  
  // Storage keys
  static const String conversationsKey = 'conversations';
  static const String themeKey = 'theme_mode';
}