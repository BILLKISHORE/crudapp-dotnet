import 'package:sagesearch/models/source.dart';

class SearchResult {
  final String query;
  final String response;
  final List<Source> sources;
  final DateTime timestamp;
  final String id;

  SearchResult({
    required this.query,
    required this.response,
    required this.sources,
    required this.timestamp,
    required this.id,
  });

  Map<String, dynamic> toJson() => {
    'query': query,
    'response': response,
    'sources': sources.map((s) => s.toJson()).toList(),
    'timestamp': timestamp.toIso8601String(),
    'id': id,
  };

  factory SearchResult.fromJson(Map<String, dynamic> json) => SearchResult(
    query: json['query'],
    response: json['response'],
    sources: (json['sources'] as List).map((s) => Source.fromJson(s)).toList(),
    timestamp: DateTime.parse(json['timestamp']),
    id: json['id'],
  );
}