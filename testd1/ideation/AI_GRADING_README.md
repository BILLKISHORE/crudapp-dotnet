# AI Grading Endpoint Documentation

## Overview

The AI Grading endpoint provides instant, intelligent grading of student answers using OpenAI's GPT models. This endpoint allows teachers and administrators to get immediate feedback on student work without storing submissions in the database.

## Features

- **Real-time Grading**: Instant AI-powered grading without database storage
- **Rubric-based Assessment**: Detailed evaluation based on custom rubrics
- **Multiple AI Models**: Support for GPT-4, GPT-3.5, Claude, and Gemini
- **Detailed Feedback**: Comprehensive feedback with strengths and improvements
- **Batch Processing**: Grade multiple submissions simultaneously
- **Confidence Scoring**: AI confidence levels for each grade
- **Rubric Analysis**: Tools to test and improve rubric effectiveness

## Endpoints

### 1. Instant Grading
**POST** `/api/v1/ai-grading/instant-grade`

Grade questions and answers instantly using AI.

#### Request Body
```json
{
  "questions": [
    {
      "question_id": "q1",
      "question_text": "Explain the concept of photosynthesis.",
      "max_marks": 10.0,
      "question_type": "descriptive",
      "rubric": {
        "definition": {
          "marks": 3,
          "description": "Clear definition of photosynthesis"
        },
        "process": {
          "marks": 4,
          "description": "Explanation of the process"
        },
        "importance": {
          "marks": 2,
          "description": "Role in ecosystems"
        },
        "examples": {
          "marks": 1,
          "description": "Relevant examples"
        }
      }
    }
  ],
  "answers": [
    {
      "question_id": "q1",
      "answer_text": "Photosynthesis is the process by which plants..."
    }
  ],
  "ai_model": "gpt-4",
  "custom_instructions": "Be thorough in feedback",
  "strict_rubric": true,
  "provide_feedback": true
}
```

#### Response
```json
{
  "success": true,
  "total_marks_obtained": 8.5,
  "total_max_marks": 10.0,
  "overall_percentage": 85.0,
  "question_grades": [
    {
      "question_id": "q1",
      "marks_obtained": 8.5,
      "max_marks": 10.0,
      "percentage": 85.0,
      "feedback": "Good explanation with clear understanding...",
      "rubric_breakdown": {
        "definition": {
          "marks_awarded": 3,
          "max_marks": 3,
          "comment": "Excellent definition"
        },
        "process": {
          "marks_awarded": 3.5,
          "max_marks": 4,
          "comment": "Good process explanation"
        }
      },
      "strengths": ["Clear definition", "Good understanding"],
      "improvements": ["Add more examples", "Include more details"]
    }
  ],
  "overall_feedback": "Strong understanding demonstrated...",
  "ai_model_used": "gpt-4",
  "confidence_score": 0.87,
  "processing_time_seconds": 2.3,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 2. Batch Grading
**POST** `/api/v1/ai-grading/batch-grade`

Grade multiple sets of questions and answers in a single request.

#### Request Body
```json
[
  {
    "questions": [...],
    "answers": [...],
    "ai_model": "gpt-4"
  },
  {
    "questions": [...],
    "answers": [...],
    "ai_model": "gpt-3.5-turbo"
  }
]
```

#### Response
```json
{
  "success": true,
  "batch_results": [...],
  "total_processing_time": 5.2,
  "successful_count": 2,
  "failed_count": 0
}
```

### 3. Available Models
**GET** `/api/v1/ai-grading/models`

Get list of available AI models and their descriptions.

#### Response
```json
{
  "success": true,
  "available_models": ["gpt-4", "gpt-3.5-turbo", "claude-3-sonnet"],
  "recommended_model": "gpt-4",
  "model_descriptions": {
    "gpt-4": "Most accurate for complex grading tasks",
    "gpt-3.5-turbo": "Faster and cost-effective for simple grading"
  }
}
```

### 4. Test Rubric Effectiveness
**POST** `/api/v1/ai-grading/test-rubric`

Analyze how effective your rubrics are for grading.

#### Request Body
```json
{
  "questions": [...],
  "answers": [...]
}
```

#### Response
```json
{
  "success": true,
  "rubric_analysis": {...},
  "recommendations": ["Add more criteria", "Improve descriptions"],
  "clarity_score": 85,
  "discrimination_score": 78
}
```

## Authentication

All endpoints require authentication with a valid JWT token:

```http
Authorization: Bearer <your_jwt_token>
```

## Usage Examples

### Python Example

```python
import requests

# Configuration
BASE_URL = "http://localhost:8000"
headers = {"Authorization": "Bearer YOUR_JWT_TOKEN"}

# Sample request
request_data = {
    "questions": [
        {
            "question_id": "q1",
            "question_text": "What is machine learning?",
            "max_marks": 5.0,
            "rubric": {
                "definition": {"marks": 2, "description": "Clear definition"},
                "applications": {"marks": 2, "description": "Real-world applications"},
                "clarity": {"marks": 1, "description": "Clear explanation"}
            }
        }
    ],
    "answers": [
        {
            "question_id": "q1",
            "answer_text": "Machine learning is a subset of AI that enables computers to learn from data without being explicitly programmed. It's used in recommendation systems, image recognition, and natural language processing."
        }
    ],
    "ai_model": "gpt-4"
}

# Send request
response = requests.post(
    f"{BASE_URL}/api/v1/ai-grading/instant-grade",
    json=request_data,
    headers=headers
)

result = response.json()
print(f"Score: {result['total_marks_obtained']}/{result['total_max_marks']}")
```

### JavaScript Example

```javascript
const API_URL = 'http://localhost:8000/api/v1/ai-grading/instant-grade';
const token = 'YOUR_JWT_TOKEN';

const requestData = {
  questions: [{
    question_id: 'q1',
    question_text: 'Explain the water cycle.',
    max_marks: 8.0,
    rubric: {
      evaporation: { marks: 2, description: 'Explain evaporation' },
      condensation: { marks: 2, description: 'Explain condensation' },
      precipitation: { marks: 2, description: 'Explain precipitation' },
      collection: { marks: 2, description: 'Explain collection' }
    }
  }],
  answers: [{
    question_id: 'q1',
    answer_text: 'The water cycle involves evaporation of water from oceans...'
  }],
  ai_model: 'gpt-4'
};

fetch(API_URL, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(requestData)
})
.then(response => response.json())
.then(data => {
  console.log('Grading Result:', data);
});
```

## Rubric Format

Rubrics should follow this structure:

```json
{
  "criteria_name": {
    "marks": 5,
    "description": "Detailed description of what this criteria evaluates"
  },
  "another_criteria": {
    "marks": 3,
    "description": "Another evaluation criteria"
  }
}
```

### Best Practices for Rubrics

1. **Clear Descriptions**: Each criteria should have a clear, specific description
2. **Balanced Marks**: Distribute marks appropriately across criteria
3. **Measurable Criteria**: Use criteria that can be objectively assessed
4. **Comprehensive Coverage**: Ensure all aspects of the answer are covered
5. **Reasonable Number**: Use 3-6 criteria for optimal clarity

## AI Models

### GPT-4
- **Best for**: Complex, nuanced grading tasks
- **Strengths**: Most accurate, detailed feedback
- **Use case**: High-stakes assessments, complex subjects

### GPT-3.5-turbo
- **Best for**: Quick, straightforward grading
- **Strengths**: Fast processing, cost-effective
- **Use case**: Simple questions, practice assessments

### Claude-3-Sonnet
- **Best for**: Detailed analysis and feedback
- **Strengths**: Thorough explanations, good at reasoning
- **Use case**: Essay grading, analytical questions

## Error Handling

The API returns structured error responses:

```json
{
  "success": false,
  "error_type": "ValidationError",
  "error_message": "Questions and answers count mismatch",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

Common error types:
- `ValidationError`: Invalid request data
- `AIGradingError`: AI service errors
- `AuthorizationError`: Authentication issues
- `InternalError`: Server errors

## Rate Limits

- **Instant Grading**: 20 questions per request
- **Batch Grading**: 10 requests per batch
- **Rate Limit**: 100 requests per minute per user

## Testing

Use the provided `ai_grading_example.py` script to test the endpoint:

```bash
python ai_grading_example.py
```

Make sure to set your JWT token in the script before running.

## Support

For issues or questions about the AI grading endpoint:

1. Check the API documentation at `/docs`
2. Review the error messages for specific issues
3. Test with the provided examples
4. Ensure your JWT token is valid and has the required permissions (Teacher/Admin)

## Changelog

### Version 1.0.0
- Initial release of AI grading endpoint
- Support for instant grading with rubrics
- Batch processing capabilities
- Multiple AI model support
- Rubric effectiveness analysis 