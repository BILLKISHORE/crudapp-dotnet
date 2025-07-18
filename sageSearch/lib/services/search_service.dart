import 'dart:math';
import 'package:sagesearch/models/search_result.dart';
import 'package:sagesearch/models/source.dart';

class SearchService {
  static SearchService? _instance;
  static SearchService get instance => _instance ??= SearchService._();
  SearchService._();

  final Random _random = Random();

  // Mock search responses
  final Map<String, List<String>> _mockResponses = {
    'ai': [
      'Artificial Intelligence continues to advance rapidly with new breakthroughs in machine learning, natural language processing, and computer vision. Recent developments include improved transformer models, better reasoning capabilities, and more efficient training methods.',
      'The field of AI is experiencing unprecedented growth, with applications spanning from healthcare and finance to autonomous vehicles and creative industries. Key trends include ethical AI development, explainable AI systems, and democratization of AI tools.',
    ],
    'climate': [
      'Climate change is causing significant shifts in global weather patterns, including more frequent extreme weather events, changing precipitation patterns, and rising sea levels. These changes are affecting ecosystems, agriculture, and human communities worldwide.',
      'Current climate research shows accelerating impacts from greenhouse gas emissions, with scientists emphasizing the need for immediate action to reduce carbon footprints and implement sustainable practices across all sectors.',
    ],
    'space': [
      'Space exploration has entered a new era with private companies joining government agencies in ambitious missions. Recent achievements include successful Mars rovers, commercial space flights, and plans for lunar bases and Mars colonization.',
      'The current state of space exploration includes active missions to Mars, Jupiter, and beyond, with new technologies enabling longer missions and more detailed scientific observations of our solar system and universe.',
    ],
    'health': [
      'Health and wellness research continues to reveal the importance of lifestyle factors in preventing disease and promoting longevity. Key areas include nutrition, exercise, mental health, and preventive care.',
      'Recent medical advances include personalized medicine, gene therapy, immunotherapy, and improved diagnostic tools that enable earlier detection and more effective treatment of diseases.',
    ],
  };

  final List<Source> _mockSources = [
    Source(
      title: 'Nature - Latest Research',
      url: 'https://nature.com/articles/research-2024',
      snippet: 'Comprehensive scientific research and peer-reviewed studies on cutting-edge topics.',
      domain: 'nature.com',
    ),
    Source(
      title: 'MIT Technology Review',
      url: 'https://technologyreview.com/2024/trends',
      snippet: 'In-depth analysis of emerging technologies and their implications for society.',
      domain: 'technologyreview.com',
    ),
    Source(
      title: 'Scientific American',
      url: 'https://scientificamerican.com/article/breakthrough',
      snippet: 'Latest scientific discoveries and breakthroughs explained for general audiences.',
      domain: 'scientificamerican.com',
    ),
    Source(
      title: 'Harvard Health Publishing',
      url: 'https://health.harvard.edu/topics/wellness',
      snippet: 'Evidence-based health information and wellness guidelines from medical experts.',
      domain: 'health.harvard.edu',
    ),
    Source(
      title: 'National Geographic',
      url: 'https://nationalgeographic.com/science/environment',
      snippet: 'Comprehensive coverage of environmental science and climate research.',
      domain: 'nationalgeographic.com',
    ),
    Source(
      title: 'NASA Science',
      url: 'https://science.nasa.gov/missions',
      snippet: 'Official NASA updates on space missions and astronomical discoveries.',
      domain: 'science.nasa.gov',
    ),
  ];

  Future<SearchResult> search(String query) async {
    // Simulate network delay
    await Future.delayed(Duration(milliseconds: 1000 + _random.nextInt(2000)));

    // Find relevant response based on query keywords
    String response = _generateResponse(query);
    
    // Select random sources (2-4 sources)
    List<Source> sources = _selectRandomSources();
    
    return SearchResult(
      query: query,
      response: response,
      sources: sources,
      timestamp: DateTime.now(),
      id: _generateId(),
    );
  }

  String _generateResponse(String query) {
    String lowerQuery = query.toLowerCase();
    
    // Check for keywords and return appropriate response
    if (lowerQuery.contains('ai') || lowerQuery.contains('artificial intelligence') || lowerQuery.contains('machine learning')) {
      return _mockResponses['ai']![_random.nextInt(_mockResponses['ai']!.length)];
    } else if (lowerQuery.contains('climate') || lowerQuery.contains('weather') || lowerQuery.contains('environment')) {
      return _mockResponses['climate']![_random.nextInt(_mockResponses['climate']!.length)];
    } else if (lowerQuery.contains('space') || lowerQuery.contains('mars') || lowerQuery.contains('exploration')) {
      return _mockResponses['space']![_random.nextInt(_mockResponses['space']!.length)];
    } else if (lowerQuery.contains('health') || lowerQuery.contains('medical') || lowerQuery.contains('wellness')) {
      return _mockResponses['health']![_random.nextInt(_mockResponses['health']!.length)];
    } else {
      // Default response for other queries
      return 'Based on current research and available information, $query is a complex topic with multiple perspectives. The latest findings suggest that understanding this subject requires considering various factors including scientific evidence, expert opinions, and ongoing developments in the field. Current trends indicate continued interest and research in this area, with new insights emerging regularly.';
    }
  }

  List<Source> _selectRandomSources() {
    List<Source> shuffled = List.from(_mockSources)..shuffle(_random);
    int count = 2 + _random.nextInt(3); // 2-4 sources
    return shuffled.take(count).toList();
  }

  String _generateId() {
    return DateTime.now().millisecondsSinceEpoch.toString() + 
           _random.nextInt(10000).toString().padLeft(4, '0');
  }
}