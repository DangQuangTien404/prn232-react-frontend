import React, { useState } from 'react';
import { Type, Image, Square, Table, Shapes, FileText, Upload } from 'lucide-react';
import useSlideStore from '@/stores/useSlideStore';
import { cn } from '@/lib/utils';

const Sidebar: React.FC = () => {
  const { addElement } = useSlideStore();
  const [activeTab, setActiveTab] = useState<'design' | 'elements' | 'text' | 'uploads'>('design');

  return (
    <div className="w-64 bg-white border-r border-gray-200 flex flex-col">
      <div className="flex justify-around p-2 border-b">
        <button onClick={() => setActiveTab('design')} className={cn('p-2', activeTab === 'design' && 'bg-gray-200')}>
          <FileText />
        </button>
        <button onClick={() => setActiveTab('elements')} className={cn('p-2', activeTab === 'elements' && 'bg-gray-200')}>
          <Shapes />
        </button>
        <button onClick={() => setActiveTab('text')} className={cn('p-2', activeTab === 'text' && 'bg-gray-200')}>
          <Type />
        </button>
        <button onClick={() => setActiveTab('uploads')} className={cn('p-2', activeTab === 'uploads' && 'bg-gray-200')}>
          <Upload />
        </button>
      </div>
      <div className="p-4">
        {activeTab === 'text' && (
          <div>
            <button onClick={() => addElement('text', 'heading')} className="w-full p-2 my-1 border">Add Heading</button>
            <button onClick={() => addElement('text', 'subheading')} className="w-full p-2 my-1 border">Add Subheading</button>
            <button onClick={() => addElement('text', 'body')} className="w-full p-2 my-1 border">Add Body</button>
          </div>
        )}
        {activeTab === 'elements' && (
          <div>
            <button onClick={() => addElement('shape')} className="w-full p-2 my-1 border">Add Shape</button>
            <button onClick={() => addElement('table')} className="w-full p-2 my-1 border">Add Table</button>
          </div>
        )}
        {activeTab === 'uploads' && (
          <div>
            <p>Upload functionality will be here.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default Sidebar;