import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:sagesearch/models/conversation.dart';
import 'package:sagesearch/utils/constants.dart';

class StorageService {
  static StorageService? _instance;
  static StorageService get instance => _instance ??= StorageService._();
  StorageService._();

  SharedPreferences? _prefs;

  Future<void> initialize() async {
    _prefs ??= await SharedPreferences.getInstance();
  }

  Future<List<Conversation>> getConversations() async {
    await initialize();
    final String? conversationsJson = _prefs?.getString(AppConstants.conversationsKey);
    if (conversationsJson == null) return [];
    
    final List<dynamic> conversationsList = jsonDecode(conversationsJson);
    return conversationsList.map((json) => Conversation.fromJson(json)).toList();
  }

  Future<void> saveConversations(List<Conversation> conversations) async {
    await initialize();
    final String conversationsJson = jsonEncode(conversations.map((c) => c.toJson()).toList());
    await _prefs?.setString(AppConstants.conversationsKey, conversationsJson);
  }

  Future<void> addConversation(Conversation conversation) async {
    final conversations = await getConversations();
    conversations.insert(0, conversation);
    await saveConversations(conversations);
  }

  Future<void> updateConversation(Conversation updatedConversation) async {
    final conversations = await getConversations();
    final index = conversations.indexWhere((c) => c.id == updatedConversation.id);
    if (index != -1) {
      conversations[index] = updatedConversation;
      await saveConversations(conversations);
    }
  }

  Future<void> deleteConversation(String conversationId) async {
    final conversations = await getConversations();
    conversations.removeWhere((c) => c.id == conversationId);
    await saveConversations(conversations);
  }

  Future<void> clearAllConversations() async {
    await initialize();
    await _prefs?.remove(AppConstants.conversationsKey);
  }
}