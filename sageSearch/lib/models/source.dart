class Source {
  final String title;
  final String url;
  final String snippet;
  final String domain;

  Source({
    required this.title,
    required this.url,
    required this.snippet,
    required this.domain,
  });

  Map<String, dynamic> toJson() => {
    'title': title,
    'url': url,
    'snippet': snippet,
    'domain': domain,
  };

  factory Source.fromJson(Map<String, dynamic> json) => Source(
    title: json['title'],
    url: json['url'],
    snippet: json['snippet'],
    domain: json['domain'],
  );
}