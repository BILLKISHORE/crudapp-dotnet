# Database Architecture Plan - School Exam Webapp

## Executive Summary

This document outlines the complete database architecture for the School Exam Webapp, designed to support 700,000+ students and teachers with scalable, secure, and efficient data storage using Supabase (PostgreSQL).

## 1. Database Schema Overview

### Core Tables Structure

#### 1.1 Authentication & User Management
- **users** - Core user information (synced with Supabase Auth)
- **user_profiles** - Extended user profile data
- **user_sessions** - Session tracking and audit
- **roles** - Role definitions (RBAC)
- **user_roles** - User-role assignments

#### 1.2 Academic Structure
- **institutions** - Schools/Organizations
- **departments** - Academic departments
- **courses** - Course/Subject definitions
- **academic_years** - Academic year management
- **enrollment** - Student-course enrollment

#### 1.3 Exam Management
- **exams** - Core exam definitions
- **exam_questions** - Question bank
- **exam_sections** - Exam sections/parts
- **exam_rubrics** - Grading criteria
- **exam_access** - Exam access control

#### 1.4 Submission & Files
- **submissions** - Student submissions
- **submission_files** - File attachments
- **submission_versions** - Version control
- **file_storage** - File metadata

#### 1.5 Grading System
- **grades** - Grading records
- **grade_components** - Detailed grade breakdown
- **grading_queue** - AI grading queue
- **grading_history** - Grade change audit

#### 1.6 Notifications & Analytics
- **notifications** - System notifications
- **audit_logs** - System audit trail
- **analytics_events** - Usage analytics
- **system_settings** - Configuration

## 2. Detailed Table Specifications

### 2.1 Core User Tables

#### users
```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    supabase_user_id UUID UNIQUE NOT NULL REFERENCES auth.users(id),
    email VARCHAR(255) UNIQUE NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    role user_role NOT NULL DEFAULT 'student',
    student_id VARCHAR(50) UNIQUE,
    department VARCHAR(100),
    institution_id UUID REFERENCES institutions(id),
    is_active BOOLEAN DEFAULT true,
    is_verified BOOLEAN DEFAULT false,
    email_verified BOOLEAN DEFAULT false,
    phone_number VARCHAR(20),
    bio TEXT,
    avatar_url TEXT,
    last_login TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

#### user_profiles
```sql
CREATE TABLE user_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    date_of_birth DATE,
    gender VARCHAR(20),
    address TEXT,
    emergency_contact JSONB,
    academic_year_id UUID REFERENCES academic_years(id),
    enrollment_date DATE,
    graduation_date DATE,
    preferences JSONB DEFAULT '{}',
    timezone VARCHAR(50) DEFAULT 'UTC',
    language VARCHAR(10) DEFAULT 'en',
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

### 2.2 Academic Structure Tables

#### institutions
```sql
CREATE TABLE institutions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    short_name VARCHAR(50),
    type institution_type DEFAULT 'school',
    address TEXT,
    contact_info JSONB,
    settings JSONB DEFAULT '{}',
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

#### departments
```sql
CREATE TABLE departments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    institution_id UUID REFERENCES institutions(id),
    name VARCHAR(255) NOT NULL,
    code VARCHAR(20) UNIQUE,
    description TEXT,
    head_id UUID REFERENCES users(id),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

#### courses
```sql
CREATE TABLE courses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    department_id UUID REFERENCES departments(id),
    code VARCHAR(20) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    credits INTEGER DEFAULT 0,
    level course_level DEFAULT 'undergraduate',
    prerequisites UUID[] DEFAULT '{}',
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

### 2.3 Exam Management Tables

#### exams
```sql
CREATE TABLE exams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    course_id UUID REFERENCES courses(id),
    creator_id UUID REFERENCES users(id) NOT NULL,
    exam_type exam_type DEFAULT 'assignment',
    
    -- Scoring Configuration
    total_marks DECIMAL(10,2) DEFAULT 100.00,
    passing_marks DECIMAL(10,2) DEFAULT 60.00,
    weight_percentage DECIMAL(5,2) DEFAULT 100.00,
    
    -- Timing Configuration
    duration_minutes INTEGER,
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    late_submission_allowed BOOLEAN DEFAULT false,
    late_penalty_percentage DECIMAL(5,2) DEFAULT 0.00,
    
    -- Access Control
    access_code VARCHAR(50),
    max_attempts INTEGER DEFAULT 1,
    shuffle_questions BOOLEAN DEFAULT false,
    show_results_immediately BOOLEAN DEFAULT false,
    
    -- Grading Configuration
    auto_grade BOOLEAN DEFAULT false,
    ai_grading_enabled BOOLEAN DEFAULT false,
    require_manual_review BOOLEAN DEFAULT true,
    
    -- Instructions and Rubrics
    instructions TEXT,
    rubric JSONB,
    grading_rubric JSONB,
    
    -- Metadata
    tags TEXT[],
    difficulty_level INTEGER DEFAULT 1,
    estimated_time_minutes INTEGER,
    
    -- Status
    status exam_status DEFAULT 'draft',
    is_published BOOLEAN DEFAULT false,
    is_active BOOLEAN DEFAULT true,
    
    -- Timestamps
    published_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

#### exam_questions
```sql
CREATE TABLE exam_questions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id UUID REFERENCES exams(id) ON DELETE CASCADE,
    question_number INTEGER NOT NULL,
    question_text TEXT NOT NULL,
    question_type question_type DEFAULT 'essay',
    
    -- Scoring
    max_marks DECIMAL(10,2) NOT NULL DEFAULT 10.00,
    
    -- MCQ/Short Answer Configuration
    options JSONB, -- For MCQ questions
    correct_answer TEXT, -- For auto-grading
    answer_key JSONB, -- Structured answer key
    
    -- Grading Configuration
    rubric_criteria JSONB,
    grading_notes TEXT,
    
    -- Media Attachments
    attachments JSONB DEFAULT '[]',
    
    -- Metadata
    tags TEXT[],
    difficulty_level INTEGER DEFAULT 1,
    estimated_time_minutes INTEGER,
    
    -- Status
    is_required BOOLEAN DEFAULT true,
    is_active BOOLEAN DEFAULT true,
    
    -- Timestamps
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now(),
    
    UNIQUE(exam_id, question_number)
);
```

### 2.4 Submission Management Tables

#### submissions
```sql
CREATE TABLE submissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id UUID REFERENCES exams(id) NOT NULL,
    student_id UUID REFERENCES users(id) NOT NULL,
    
    -- Submission Details
    submission_text TEXT,
    answers JSONB DEFAULT '{}', -- Structured answers
    
    -- File Management
    file_count INTEGER DEFAULT 0,
    total_file_size BIGINT DEFAULT 0,
    
    -- Timing Information
    started_at TIMESTAMPTZ,
    submitted_at TIMESTAMPTZ DEFAULT now(),
    time_taken_minutes INTEGER,
    
    -- Status Management
    status submission_status DEFAULT 'submitted',
    attempt_number INTEGER DEFAULT 1,
    is_late BOOLEAN DEFAULT false,
    late_penalty_applied DECIMAL(5,2) DEFAULT 0.00,
    
    -- AI Processing
    ai_processing_status ai_processing_status DEFAULT 'pending',
    ai_processing_started_at TIMESTAMPTZ,
    ai_processing_completed_at TIMESTAMPTZ,
    ai_processing_error TEXT,
    ai_confidence_score DECIMAL(5,4),
    
    -- Metadata
    ip_address INET,
    user_agent TEXT,
    browser_info JSONB,
    
    -- Academic Integrity
    plagiarism_score DECIMAL(5,4),
    similarity_report JSONB,
    
    -- Timestamps
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now(),
    
    UNIQUE(exam_id, student_id, attempt_number)
);
```

#### submission_files
```sql
CREATE TABLE submission_files (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    submission_id UUID REFERENCES submissions(id) ON DELETE CASCADE,
    
    -- File Information
    file_name VARCHAR(255) NOT NULL,
    original_name VARCHAR(255) NOT NULL,
    file_path TEXT NOT NULL,
    file_url TEXT,
    file_size BIGINT NOT NULL,
    file_type VARCHAR(100) NOT NULL,
    mime_type VARCHAR(100),
    
    -- File Processing
    processing_status file_processing_status DEFAULT 'pending',
    processing_error TEXT,
    extracted_text TEXT,
    
    -- Security
    file_hash VARCHAR(128),
    is_encrypted BOOLEAN DEFAULT false,
    
    -- Metadata
    file_order INTEGER DEFAULT 1,
    is_primary BOOLEAN DEFAULT false,
    
    -- Timestamps
    uploaded_at TIMESTAMPTZ DEFAULT now(),
    processed_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

### 2.5 Grading System Tables

#### grades
```sql
CREATE TABLE grades (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    submission_id UUID REFERENCES submissions(id) NOT NULL,
    grader_id UUID REFERENCES users(id) NOT NULL,
    question_id UUID REFERENCES exam_questions(id),
    
    -- Scoring
    score DECIMAL(10,2),
    max_score DECIMAL(10,2) NOT NULL,
    percentage DECIMAL(5,2),
    
    -- Grade Details
    grade_type grade_type NOT NULL,
    status grade_status DEFAULT 'draft',
    is_final BOOLEAN DEFAULT false,
    
    -- Detailed Scoring
    component_scores JSONB DEFAULT '{}',
    rubric_scores JSONB DEFAULT '{}',
    
    -- Penalties and Bonuses
    late_penalty DECIMAL(10,2) DEFAULT 0.00,
    bonus_points DECIMAL(10,2) DEFAULT 0.00,
    
    -- Feedback
    feedback TEXT,
    private_notes TEXT,
    public_comments TEXT,
    
    -- AI Grading Information
    ai_model_used VARCHAR(100),
    ai_confidence_score DECIMAL(5,4),
    ai_processing_time_ms INTEGER,
    ai_raw_response JSONB,
    
    -- Review Process
    reviewed_by UUID REFERENCES users(id),
    reviewed_at TIMESTAMPTZ,
    review_comments TEXT,
    requires_review BOOLEAN DEFAULT false,
    
    -- Timestamps
    graded_at TIMESTAMPTZ DEFAULT now(),
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

### 2.6 System Tables

#### notifications
```sql
CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    
    -- Notification Content
    type notification_type NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    
    -- Context
    context_type VARCHAR(50), -- exam, submission, grade, etc.
    context_id UUID,
    related_user_id UUID REFERENCES users(id),
    
    -- Delivery
    channels notification_channel[] DEFAULT '{in_app}',
    priority notification_priority DEFAULT 'normal',
    
    -- Status
    is_read BOOLEAN DEFAULT false,
    read_at TIMESTAMPTZ,
    is_sent BOOLEAN DEFAULT false,
    sent_at TIMESTAMPTZ,
    
    -- Scheduling
    scheduled_for TIMESTAMPTZ,
    expires_at TIMESTAMPTZ,
    
    -- Metadata
    metadata JSONB DEFAULT '{}',
    
    -- Timestamps
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);
```

#### audit_logs
```sql
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    
    -- User Context
    user_id UUID REFERENCES users(id),
    session_id UUID,
    
    -- Action Details
    action VARCHAR(100) NOT NULL,
    resource_type VARCHAR(50) NOT NULL,
    resource_id UUID,
    
    -- Change Details
    old_values JSONB,
    new_values JSONB,
    changes JSONB,
    
    -- Request Context
    ip_address INET,
    user_agent TEXT,
    method VARCHAR(10),
    endpoint TEXT,
    
    -- Metadata
    metadata JSONB DEFAULT '{}',
    severity log_severity DEFAULT 'info',
    
    -- Timestamps
    created_at TIMESTAMPTZ DEFAULT now()
);
```

## 3. Custom Types and Enums

```sql
-- User and Role Types
CREATE TYPE user_role AS ENUM ('student', 'teacher', 'admin', 'super_admin');
CREATE TYPE institution_type AS ENUM ('school', 'college', 'university', 'training_center');
CREATE TYPE course_level AS ENUM ('elementary', 'middle', 'high', 'undergraduate', 'graduate');

-- Exam Types
CREATE TYPE exam_type AS ENUM ('quiz', 'test', 'midterm', 'final', 'assignment', 'project');
CREATE TYPE exam_status AS ENUM ('draft', 'scheduled', 'active', 'completed', 'cancelled');
CREATE TYPE question_type AS ENUM ('essay', 'short_answer', 'multiple_choice', 'true_false', 'matching', 'fill_blank');

-- Submission Types
CREATE TYPE submission_status AS ENUM ('draft', 'submitted', 'grading', 'graded', 'returned');
CREATE TYPE ai_processing_status AS ENUM ('pending', 'processing', 'completed', 'failed', 'skipped');
CREATE TYPE file_processing_status AS ENUM ('pending', 'processing', 'completed', 'failed');

-- Grading Types
CREATE TYPE grade_type AS ENUM ('manual', 'ai_generated', 'ai_assisted', 'auto_graded');
CREATE TYPE grade_status AS ENUM ('draft', 'provisional', 'final', 'appealed');

-- System Types
CREATE TYPE notification_type AS ENUM ('exam_created', 'exam_due', 'submission_received', 'graded', 'grade_updated');
CREATE TYPE notification_channel AS ENUM ('email', 'sms', 'push', 'in_app');
CREATE TYPE notification_priority AS ENUM ('low', 'normal', 'high', 'urgent');
CREATE TYPE log_severity AS ENUM ('debug', 'info', 'warning', 'error', 'critical');
```

## 4. Indexes for Performance

```sql
-- User table indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_student_id ON users(student_id);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_active ON users(is_active);

-- Exam table indexes
CREATE INDEX idx_exams_creator ON exams(creator_id);
CREATE INDEX idx_exams_course ON exams(course_id);
CREATE INDEX idx_exams_status ON exams(status);
CREATE INDEX idx_exams_dates ON exams(start_date, end_date);

-- Submission table indexes
CREATE INDEX idx_submissions_exam_student ON submissions(exam_id, student_id);
CREATE INDEX idx_submissions_student ON submissions(student_id);
CREATE INDEX idx_submissions_status ON submissions(status);
CREATE INDEX idx_submissions_ai_processing ON submissions(ai_processing_status);

-- Grade table indexes
CREATE INDEX idx_grades_submission ON grades(submission_id);
CREATE INDEX idx_grades_grader ON grades(grader_id);
CREATE INDEX idx_grades_final ON grades(is_final);

-- Notification indexes
CREATE INDEX idx_notifications_user ON notifications(user_id);
CREATE INDEX idx_notifications_unread ON notifications(user_id, is_read);

-- Audit log indexes
CREATE INDEX idx_audit_logs_user ON audit_logs(user_id);
CREATE INDEX idx_audit_logs_resource ON audit_logs(resource_type, resource_id);
CREATE INDEX idx_audit_logs_created ON audit_logs(created_at);
```

## 5. Row Level Security (RLS) Policies

### 5.1 Users Table RLS
```sql
-- Users can only see their own data
CREATE POLICY users_select_own ON users
    FOR SELECT USING (auth.uid() = supabase_user_id);

-- Users can update their own data
CREATE POLICY users_update_own ON users
    FOR UPDATE USING (auth.uid() = supabase_user_id);

-- Teachers can see their students
CREATE POLICY users_teachers_see_students ON users
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM users u
            WHERE u.supabase_user_id = auth.uid()
            AND u.role = 'teacher'
        )
        AND role = 'student'
    );
```

### 5.2 Exams Table RLS
```sql
-- Students can only see published exams
CREATE POLICY exams_students_published ON exams
    FOR SELECT USING (
        is_published = true
        AND status = 'active'
        AND EXISTS (
            SELECT 1 FROM users u
            WHERE u.supabase_user_id = auth.uid()
            AND u.role = 'student'
        )
    );

-- Teachers can see their own exams
CREATE POLICY exams_teachers_own ON exams
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM users u
            WHERE u.supabase_user_id = auth.uid()
            AND u.id = creator_id
        )
    );
```

### 5.3 Submissions Table RLS
```sql
-- Students can only see their own submissions
CREATE POLICY submissions_students_own ON submissions
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM users u
            WHERE u.supabase_user_id = auth.uid()
            AND u.id = student_id
        )
    );

-- Teachers can see submissions for their exams
CREATE POLICY submissions_teachers_exams ON submissions
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM exams e
            JOIN users u ON u.id = e.creator_id
            WHERE u.supabase_user_id = auth.uid()
            AND e.id = exam_id
        )
    );
```

## 6. Database Functions and Triggers

### 6.1 Update Timestamps
```sql
-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply to all tables with updated_at column
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_exams_updated_at BEFORE UPDATE ON exams
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- ... (apply to all relevant tables)
```

### 6.2 Grade Calculation
```sql
-- Function to calculate final grade
CREATE OR REPLACE FUNCTION calculate_final_grade(submission_uuid UUID)
RETURNS DECIMAL(10,2) AS $$
DECLARE
    total_score DECIMAL(10,2) := 0;
    max_possible DECIMAL(10,2) := 0;
    final_percentage DECIMAL(10,2);
BEGIN
    SELECT 
        COALESCE(SUM(g.score), 0),
        COALESCE(SUM(g.max_score), 0)
    INTO total_score, max_possible
    FROM grades g
    WHERE g.submission_id = submission_uuid
    AND g.is_final = true;
    
    IF max_possible > 0 THEN
        final_percentage := (total_score / max_possible) * 100;
    ELSE
        final_percentage := 0;
    END IF;
    
    RETURN final_percentage;
END;
$$ LANGUAGE plpgsql;
```

## 7. Views for Common Queries

### 7.1 Student Dashboard View
```sql
CREATE VIEW student_dashboard AS
SELECT 
    u.id as student_id,
    u.full_name,
    e.id as exam_id,
    e.title as exam_title,
    e.end_date,
    e.total_marks,
    s.id as submission_id,
    s.status as submission_status,
    s.submitted_at,
    COALESCE(g.percentage, 0) as grade_percentage,
    CASE 
        WHEN s.id IS NULL THEN 'not_started'
        WHEN s.status = 'submitted' AND g.id IS NULL THEN 'pending_grade'
        WHEN g.id IS NOT NULL THEN 'graded'
    END as overall_status
FROM users u
CROSS JOIN exams e
LEFT JOIN submissions s ON s.student_id = u.id AND s.exam_id = e.id
LEFT JOIN grades g ON g.submission_id = s.id AND g.is_final = true
WHERE u.role = 'student'
AND e.is_published = true
AND e.status = 'active';
```

### 7.2 Teacher Grading Queue View
```sql
CREATE VIEW teacher_grading_queue AS
SELECT 
    t.id as teacher_id,
    e.id as exam_id,
    e.title as exam_title,
    s.id as submission_id,
    st.full_name as student_name,
    s.submitted_at,
    s.ai_processing_status,
    CASE 
        WHEN g.id IS NULL THEN 'ungraded'
        WHEN g.status = 'draft' THEN 'draft'
        WHEN g.status = 'final' THEN 'completed'
    END as grading_status,
    EXTRACT(EPOCH FROM (now() - s.submitted_at))/3600 as hours_since_submission
FROM users t
JOIN exams e ON e.creator_id = t.id
JOIN submissions s ON s.exam_id = e.id
JOIN users st ON st.id = s.student_id
LEFT JOIN grades g ON g.submission_id = s.id AND g.is_final = true
WHERE t.role = 'teacher'
AND s.status = 'submitted'
ORDER BY s.submitted_at DESC;
```

## 8. Implementation Steps

### Step 1: Environment Setup
1. Verify Supabase connection
2. Create development database
3. Set up migration system
4. Configure backup strategy

### Step 2: Core Schema Creation
1. Create custom types and enums
2. Create core tables (users, institutions, departments)
3. Set up authentication integration
4. Implement basic RLS policies

### Step 3: Academic Structure
1. Create courses and academic year tables
2. Implement enrollment system
3. Set up course-user relationships
4. Create academic hierarchy

### Step 4: Exam Management
1. Create exam and question tables
2. Implement exam configuration
3. Set up rubric system
4. Create exam access controls

### Step 5: Submission System
1. Create submission tables
2. Implement file storage integration
3. Set up version control
4. Create submission validation

### Step 6: Grading System
1. Create grading tables
2. Implement AI grading queue
3. Set up manual grading workflow
4. Create grade calculation functions

### Step 7: System Features
1. Create notification system
2. Implement audit logging
3. Set up analytics tracking
4. Create system monitoring

### Step 8: Performance Optimization
1. Create indexes
2. Optimize queries
3. Set up caching strategy
4. Implement query monitoring

### Step 9: Security Hardening
1. Complete RLS implementation
2. Set up data encryption
3. Implement access controls
4. Create security monitoring

### Step 10: Testing and Validation
1. Create test data
2. Performance testing
3. Security testing
4. User acceptance testing

## 9. Migration Strategy

### Phase 1: Foundation (Week 1-2)
- Core user tables and authentication
- Basic institution structure
- Essential RLS policies

### Phase 2: Academic Core (Week 3-4)
- Course and exam management
- Submission system
- File storage integration

### Phase 3: Advanced Features (Week 5-6)
- Grading system
- AI integration
- Notification system

### Phase 4: Optimization (Week 7-8)
- Performance tuning
- Security hardening
- Monitoring setup

## 10. Monitoring and Maintenance

### Performance Monitoring
- Query performance tracking
- Index usage analysis
- Connection pool monitoring
- Resource utilization

### Data Integrity
- Constraint validation
- Referential integrity checks
- Data quality monitoring
- Backup verification

### Security Monitoring
- Access pattern analysis
- Unusual activity detection
- RLS policy effectiveness
- Audit log analysis

This comprehensive database architecture provides a solid foundation for your school exam webapp, supporting all the features outlined in your project requirements while ensuring scalability, security, and performance. 