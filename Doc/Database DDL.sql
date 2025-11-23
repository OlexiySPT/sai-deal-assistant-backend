-- Enable uuid-ossp or use gen_random_uuid() from pgcrypto depending on your PG setup.
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Users
CREATE TABLE app_user (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  username TEXT UNIQUE NOT NULL,
  email TEXT UNIQUE NOT NULL,
  password_hash TEXT NOT NULL,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  last_login TIMESTAMP WITH TIME ZONE
);

-- Roles (simple RBAC)
CREATE TABLE role (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL UNIQUE,
  description TEXT
);
CREATE TABLE user_role (
  user_id UUID REFERENCES app_user(id) ON DELETE CASCADE,
  role_id UUID REFERENCES role(id) ON DELETE CASCADE,
  PRIMARY KEY (user_id, role_id)
);

-- Employee (linked to user)
CREATE TABLE employee (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID UNIQUE REFERENCES app_user(id) ON DELETE SET NULL,
  first_name TEXT NOT NULL,
  last_name TEXT NOT NULL,
  title TEXT,
  phone TEXT,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Contragent (potential client)
CREATE TABLE contragent (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL,
  industry TEXT,
  status TEXT DEFAULT 'new', -- e.g., new, contacted, negotiating, won, lost
  responsible_employee_id UUID REFERENCES employee(id) ON DELETE SET NULL,
  description TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Contact representatives for contragent
CREATE TABLE contact_rep (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  contragent_id UUID REFERENCES contragent(id) ON DELETE CASCADE,
  first_name TEXT NOT NULL,
  last_name TEXT NOT NULL,
  title TEXT,
  email TEXT,
  phone TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Event types (customizable)
CREATE TABLE event_type (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL UNIQUE,
  description TEXT,
  created_by UUID REFERENCES app_user(id),
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Events
CREATE TABLE event (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  contragent_id UUID REFERENCES contragent(id) ON DELETE CASCADE,
  responsible_employee_id UUID REFERENCES employee(id) ON DELETE SET NULL,
  event_type_id UUID REFERENCES event_type(id) ON DELETE SET NULL,
  scheduled_at TIMESTAMP WITH TIME ZONE,
  status TEXT DEFAULT 'planned', -- planned, done, cancelled
  description TEXT,
  result TEXT,
  additional_data JSONB, -- flexible custom fields
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Link table for Event <-> ContactRep (one event may reference multiple contact reps)
CREATE TABLE event_contact_rep (
  event_id UUID REFERENCES event(id) ON DELETE CASCADE,
  contact_rep_id UUID REFERENCES contact_rep(id) ON DELETE CASCADE,
  PRIMARY KEY (event_id, contact_rep_id)
);

-- Attachments table: generic attachments for contragent or event.
CREATE TABLE attachment (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  owner_type TEXT NOT NULL, -- 'contragent' | 'event'
  owner_id UUID NOT NULL,
  filename TEXT NOT NULL,
  content_type TEXT,
  size_bytes BIGINT,
  uploaded_by UUID REFERENCES app_user(id) ON DELETE SET NULL,
  uploaded_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  -- To keep referential integrity, create application-level checks or partial FK constraints per owner_type
  metadata JSONB
);

-- Optional: simple audit log
CREATE TABLE audit_log (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES app_user(id) ON DELETE SET NULL,
  entity_type TEXT,
  entity_id UUID,
  action TEXT,
  payload JSONB,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Indexes for performance
CREATE INDEX idx_contragent_responsible ON contragent(responsible_employee_id);
CREATE INDEX idx_event_responsible ON event(responsible_employee_id);
CREATE INDEX idx_event_scheduled_at ON event(scheduled_at);
CREATE INDEX idx_event_type ON event(event_type_id);
CREATE INDEX idx_attachment_owner ON attachment(owner_type, owner_id);
