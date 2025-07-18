# SageSearch - Perplexity Clone Architecture

## Overview
SageSearch is a modern, AI-powered search application that provides conversational search experiences similar to Perplexity. The app focuses on delivering clean, intuitive interfaces with intelligent search capabilities.

## Core Features (MVP)
1. **Search Interface**: Clean, minimal search input with modern design
2. **Conversation View**: Chat-like interface for search queries and AI responses
3. **Source Citations**: Display sources and references for search results
4. **Search History**: Local storage of previous searches and conversations
5. **Theme Support**: Light/dark mode with smooth transitions
6. **Suggested Questions**: Pre-populated search suggestions

## Technical Architecture

### File Structure
```
lib/
├── main.dart                    # App entry point
├── theme.dart                   # Theme configurations (existing)
├── models/
│   ├── search_result.dart       # Search result data model
│   ├── conversation.dart        # Conversation data model
│   └── source.dart              # Source citation model
├── screens/
│   ├── home_screen.dart         # Main search interface
│   ├── chat_screen.dart         # Conversation/results view
│   └── history_screen.dart      # Search history
├── widgets/
│   ├── search_bar.dart          # Custom search input
│   ├── message_bubble.dart      # Chat message bubble
│   ├── source_card.dart         # Source citation card
│   └── suggested_questions.dart # Suggested search topics
├── services/
│   ├── search_service.dart      # Mock search API service
│   └── storage_service.dart     # Local data persistence
└── utils/
    └── constants.dart           # App constants
```

### Data Models
- **SearchResult**: Contains query, response, sources, timestamp
- **Conversation**: Manages chat-like search sessions
- **Source**: Represents web sources with titles, URLs, snippets

### Key Components
1. **HomePage**: Landing page with search input and suggestions
2. **ChatScreen**: Conversation interface for search results
3. **SearchBar**: Custom search input with modern design
4. **MessageBubble**: Chat-style message display
5. **SourceCard**: Displays source citations with links

### Local Storage
- Use SharedPreferences for search history
- JSON serialization for complex data structures
- Persist conversations and user preferences

## Design Principles
- **Modern UI**: Clean, minimal design with subtle animations
- **Conversational**: Chat-like interface for natural interaction
- **Fast**: Immediate feedback and smooth transitions
- **Accessible**: High contrast, proper text sizes, screen reader support
- **Responsive**: Works well on different screen sizes

## Implementation Steps
1. Update main.dart with proper title and HomePage
2. Create data models for search results and conversations
3. Implement HomePage with search interface and suggestions
4. Build ChatScreen for displaying search conversations
5. Create reusable UI components (SearchBar, MessageBubble, etc.)
6. Add local storage service for history persistence
7. Implement mock search service with realistic data
8. Add animations and transitions for better UX
9. Test and fix any compilation errors

## Color Palette
The existing theme provides a purple-based color scheme:
- Primary: Deep purple (#684F8E)
- Secondary: Muted purple-gray (#635D70)
- Accent: Soft purple (#D4BCCF)
- Background: Clean whites/dark surfaces

## Animation Strategy
- Smooth page transitions
- Typing indicators for search responses
- Subtle hover effects on interactive elements
- Loading states with progress indicators