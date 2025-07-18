import 'package:flutter/material.dart';
import 'package:sagesearch/models/conversation.dart';
import 'package:sagesearch/models/search_result.dart';
import 'package:sagesearch/screens/chat_screen.dart';
import 'package:sagesearch/screens/history_screen.dart';
import 'package:sagesearch/screens/profile_screen.dart';
import 'package:sagesearch/services/search_service.dart';
import 'package:sagesearch/services/storage_service.dart';
import 'package:sagesearch/utils/constants.dart';
import 'dart:math' as math;

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> with TickerProviderStateMixin {
  final TextEditingController _searchController = TextEditingController();
  final SearchService _searchService = SearchService.instance;
  final StorageService _storageService = StorageService.instance;

  bool _isLoading = false;
  late AnimationController _fadeAnimationController;
  late Animation<double> _fadeAnimation;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
    _storageService.initialize();
  }

  void _initializeAnimations() {
    _fadeAnimationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );
    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _fadeAnimationController,
      curve: Curves.easeInOut,
    ));

    _fadeAnimationController.forward();
  }

  @override
  void dispose() {
    _searchController.dispose();
    _fadeAnimationController.dispose();
    super.dispose();
  }

  Future<void> _performSearch([String? query]) async {
    final searchQuery = query ?? _searchController.text.trim();
    if (searchQuery.isEmpty) return;

    setState(() => _isLoading = true);

    try {
      final SearchResult result = await _searchService.search(searchQuery);

      // Create new conversation
      final conversation = Conversation(
        id: result.id,
        title: _generateConversationTitle(searchQuery),
        messages: [result],
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
      );

      // Save to storage
      await _storageService.addConversation(conversation);

      // Navigate to chat screen
      if (mounted) {
        Navigator.of(context).push(
          PageRouteBuilder(
            pageBuilder: (context, animation, secondaryAnimation) =>
                ChatScreen(conversation: conversation),
            transitionsBuilder:
                (context, animation, secondaryAnimation, child) {
              return SlideTransition(
                position: animation.drive(
                  Tween(begin: const Offset(1.0, 0.0), end: Offset.zero)
                      .chain(CurveTween(curve: Curves.easeInOut)),
                ),
                child: child,
              );
            },
            transitionDuration: AppConstants.mediumAnimation,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Error: ${e.toString()}'),
            backgroundColor: Theme.of(context).colorScheme.error,
          ),
        );
      }
    } finally {
      setState(() => _isLoading = false);
      _searchController.clear();
    }
  }

  String _generateConversationTitle(String query) {
    if (query.length <= 50) return query;
    return '${query.substring(0, 47)}...';
  }

  void _navigateToHistory() {
    Navigator.of(context).push(
      PageRouteBuilder(
        pageBuilder: (context, animation, secondaryAnimation) =>
            const HistoryScreen(),
        transitionsBuilder: (context, animation, secondaryAnimation, child) {
          return SlideTransition(
            position: animation.drive(
              Tween(begin: const Offset(0.0, 1.0), end: Offset.zero)
                  .chain(CurveTween(curve: Curves.easeInOut)),
            ),
            child: child,
          );
        },
        transitionDuration: AppConstants.mediumAnimation,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      backgroundColor: const Color(0xFF1a1a1a),
      body: Stack(
        children: [
          // Geometric background pattern
          _buildGeometricBackground(),

          // Status bar
          SafeArea(
            child: Column(
              children: [
                _buildStatusBar(),

                // Main content
                Expanded(
                  child: FadeTransition(
                    opacity: _fadeAnimation,
                    child: Column(
                      children: [
                        const Spacer(flex: 2),

                        // Central compass icon
                        _buildCompassIcon(),

                        const SizedBox(height: 32),

                        // "Where knowledge begins" text
                        _buildTagline(),

                        const Spacer(flex: 3),

                        // Search bar at bottom
                        _buildSearchBar(),

                        const SizedBox(height: 16),
                      ],
                    ),
                  ),
                ),

                // Bottom navigation
                _buildBottomNavigation(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildGeometricBackground() {
    return Container(
      width: double.infinity,
      height: double.infinity,
      decoration: const BoxDecoration(
        color: Color(0xFF1a1a1a),
      ),
      child: CustomPaint(
        painter: GeometricPatternPainter(),
      ),
    );
  }

  Widget _buildStatusBar() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          // User avatar
          GestureDetector(
            onTap: () {
              Navigator.of(context).push(
                PageRouteBuilder(
                  pageBuilder: (context, animation, secondaryAnimation) =>
                      const ProfileScreen(),
                  transitionsBuilder: (context, animation, secondaryAnimation, child) {
                    return SlideTransition(
                      position: animation.drive(
                        Tween(begin: const Offset(-1.0, 0.0), end: Offset.zero)
                            .chain(CurveTween(curve: Curves.easeInOut)),
                      ),
                      child: child,
                    );
                  },
                  transitionDuration: AppConstants.mediumAnimation,
                ),
              );
            },
            child: Container(
              width: 32,
              height: 32,
              decoration: BoxDecoration(
                color: Colors.grey[800],
                borderRadius: BorderRadius.circular(16),
              ),
              child: const Icon(
                Icons.person,
                color: Colors.white,
                size: 20,
              ),
            ),
          ),

          // Airtel | Perplexity PRO
          Row(
            children: [
              Image.asset(
                'assets/images/airtel_logo.png',
                height: 20,
                width: 20,
                errorBuilder: (context, error, stackTrace) => Container(
                  width: 20,
                  height: 20,
                  decoration: BoxDecoration(
                    color: Colors.red,
                    borderRadius: BorderRadius.circular(4),
                  ),
                ),
              ),
              const SizedBox(width: 4),
              const Text(
                " ",
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                ),
              ),
              const SizedBox(width: 8),
              const Text(
                '|',
                style: TextStyle(
                  color: Colors.grey,
                  fontSize: 16,
                ),
              ),
              const SizedBox(width: 8),
              const Text(
                "perplexiHype",
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                ),
              ),
              Container(
                margin: const EdgeInsets.only(left: 4),
                padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 2),
                decoration: BoxDecoration(
                  color: Colors.orange,
                  borderRadius: BorderRadius.circular(4),
                ),
                child: const Text(
                  'PRO',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 10,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),

          // Share button
          const Icon(
            Icons.share,
            color: Colors.white,
            size: 24,
          ),
        ],
      ),
    );
  }

  Widget _buildCompassIcon() {
    return Container(
      width: 80,
      height: 80,
      decoration: BoxDecoration(
        color: Colors.grey[800]?.withValues(alpha: 0.3),
        borderRadius: BorderRadius.circular(8),
      ),
      child: const Icon(
        Icons.navigation,
        color: Colors.white,
        size: 40,
      ),
    );
  }

  Widget _buildTagline() {
    return const Column(
      children: [
        Text(
          'Where',
          style: TextStyle(
            color: Colors.grey,
            fontSize: 36,
            fontWeight: FontWeight.w300,
          ),
        ),
        Text(
          'knowledge',
          style: TextStyle(
            color: Colors.grey,
            fontSize: 36,
            fontWeight: FontWeight.w300,
          ),
        ),
        Text(
          'begins',
          style: TextStyle(
            color: Colors.grey,
            fontSize: 36,
            fontWeight: FontWeight.w300,
          ),
        ),
      ],
    );
  }

  Widget _buildSearchBar() {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 20),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.grey[900],
        borderRadius: BorderRadius.circular(25),
      ),
      child: Row(
        children: [
          const Icon(
            Icons.camera_alt,
            color: Colors.grey,
            size: 20,
          ),
          const SizedBox(width: 12),
          Expanded(
            child: TextField(
              controller: _searchController,
              style: const TextStyle(
                color: Colors.white,
                fontSize: 16,
              ),
              decoration: const InputDecoration(
                hintText: 'Ask anything...',
                hintStyle: TextStyle(
                  color: Colors.grey,
                  fontSize: 16,
                ),
                border: InputBorder.none,
              ),
              onSubmitted: (query) => _performSearch(query),
            ),
          ),
          const SizedBox(width: 12),
          const Icon(
            Icons.graphic_eq,
            color: Colors.grey,
            size: 20,
          ),
        ],
      ),
    );
  }

  Widget _buildBottomNavigation() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 16),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          _buildNavItem(Icons.search, 0),
          _buildNavItem(Icons.public, 1),
          _buildNavItem(Icons.star_border, 2),
          _buildNavItem(Icons.notifications_none, 3),
        ],
      ),
    );
  }

  Widget _buildNavItem(IconData icon, int index) {
    final isSelected = _selectedIndex == index;
    return GestureDetector(
      onTap: () {
        setState(() {
          _selectedIndex = index;
        });

        // Handle navigation based on index
        switch (index) {
          case 0:
            // Search - already on home
            break;
          case 1:
            // Globe - navigate to DiscoverPage
            Navigator.of(context).push(
              MaterialPageRoute(builder: (context) => DiscoverPage()),
            );
            break;
          case 2:
            // Star - could navigate to favorites
            break;
          case 3:
            // Notifications
            break;
        }
      },
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: isSelected ? Colors.grey[800] : Colors.transparent,
          borderRadius: BorderRadius.circular(8),
        ),
        child: Icon(
          icon,
          color: isSelected ? Colors.white : Colors.grey,
          size: 24,
        ),
      ),
    );
  }
}

class GeometricPatternPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.grey.withValues(alpha: 0.1)
      ..strokeWidth = 1
      ..style = PaintingStyle.stroke;

    final centerX = size.width / 2;
    final centerY = size.height / 2;

    // Draw geometric lines
    for (int i = 0; i < 8; i++) {
      final angle = (i * 45) * (3.14159 / 180);
      final endX = centerX + (size.width * 0.7) * math.cos(angle);
      final endY = centerY + (size.height * 0.7) * math.sin(angle);

      canvas.drawLine(
        Offset(centerX, centerY),
        Offset(endX, endY),
        paint,
      );
    }

    // Draw concentric circles
    for (int i = 1; i <= 3; i++) {
      canvas.drawCircle(
        Offset(centerX, centerY),
        (size.width / 6) * i,
        paint,
      );
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class DiscoverPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      appBar: AppBar(
        backgroundColor: Colors.black,
        elevation: 0,
        title: Text('Discover', style: TextStyle(color: Colors.white)),
        actions: [
          Icon(Icons.headphones, color: Colors.white),
          SizedBox(width: 16),
          Icon(Icons.favorite_border, color: Colors.white),
          SizedBox(width: 16),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Tabs
            Row(
              children: [
                _TabButton(text: "For You", selected: true),
                _TabButton(text: "Top Stories"),
                _TabButton(text: "Tech & Science"),
                // Add more tabs as needed
              ],
            ),
            SizedBox(height: 24),
            // News Card
            Card(
              color: Color(0xFF2C2C2C),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(24),
              ),
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  children: [
                    // Image
                    ClipRRect(
                      borderRadius: BorderRadius.circular(16),
                      child: Image.network(
                        'https://upload.wikimedia.org/wikipedia/commons/5/5c/Vladimir_Putin_%282021-03-10%29_%28cropped%29.jpg',
                        height: 200,
                        width: double.infinity,
                        fit: BoxFit.cover,
                      ),
                    ),
                    SizedBox(height: 16),
                    // Title
                    Text(
                      "Russia warns WhatsApp to prepare for exit from country",
                      style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                        fontSize: 20,
                      ),
                    ),
                    SizedBox(height: 8),
                    // Subtitle
                    Text(
                      "Russian lawmakers delivered a stark warning to WhatsApp on Friday, telling th...",
                      style: TextStyle(
                        color: Colors.grey[400],
                        fontSize: 16,
                      ),
                    ),
                    SizedBox(height: 16),
                    // Footer
                    Row(
                      children: [
                        CircleAvatar(
                          backgroundImage: NetworkImage('https://api.dicebear.com/7.x/miniavs/svg?seed=alexwanders'),
                          radius: 12,
                        ),
                        SizedBox(width: 8),
                        Text(
                          "alexwanders",
                          style: TextStyle(color: Colors.white),
                        ),
                        Spacer(),
                        Icon(Icons.bookmark_border, color: Colors.white),
                        SizedBox(width: 16),
                        Icon(Icons.headphones, color: Colors.white),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _TabButton extends StatelessWidget {
  final String text;
  final bool selected;
  const _TabButton({required this.text, this.selected = false});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(right: 12.0),
      child: Container(
        decoration: BoxDecoration(
          color: selected ? Colors.teal[900] : Colors.transparent,
          borderRadius: BorderRadius.circular(16),
        ),
        padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        child: Text(
          text,
          style: TextStyle(
            color: Colors.white,
            fontWeight: selected ? FontWeight.bold : FontWeight.normal,
          ),
        ),
      ),
    );
  }
}
