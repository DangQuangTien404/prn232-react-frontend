import create from 'zustand';
import { Slide, SlideElement } from '@/types/slide';

interface SlideState {
  slides: Slide[];
  currentSlideIndex: number;
  selectedElement: string | null;
  history: Slide[][];
  historyIndex: number;
  setSlides: (slides: Slide[]) => void;
  setCurrentSlideIndex: (index: number) => void;
  setSelectedElement: (elementId: string | null) => void;
  addElement: (type: SlideElement['type'], textStyle?: 'heading' | 'subheading' | 'body', symbol?: string) => void;
  updateElementStyle: (elementId: string, property: string, value: any) => void;
  deleteElement: (elementId: string) => void;
  undo: () => void;
  redo: () => void;
  saveToHistory: (slides: Slide[]) => void;
}

const useSlideStore = create<SlideState>((set, get) => ({
  slides: [
    {
      id: '1',
      elements: [],
      backgroundColor: '#ffffff',
    },
  ],
  currentSlideIndex: 0,
  selectedElement: null,
  history: [
    [
      {
        id: '1',
        elements: [],
        backgroundColor: '#ffffff',
      },
    ],
  ],
  historyIndex: 0,

  setSlides: (slides) => set({ slides }),
  setCurrentSlideIndex: (index) => set({ currentSlideIndex: index }),
  setSelectedElement: (elementId) => set({ selectedElement: elementId }),

  saveToHistory: (slides) => {
    const { history, historyIndex } = get();
    const newHistory = history.slice(0, historyIndex + 1);
    newHistory.push(slides);
    set({ history: newHistory, historyIndex: newHistory.length - 1 });
  },

  addElement: (type, textStyle, symbol) => {
    const { slides, currentSlideIndex, saveToHistory } = get();
    let content = '';
    let fontSize = 16;
    let fontWeight: 'normal' | 'bold' = 'normal';

    if (type === 'text') {
      if (symbol) {
        content = symbol;
        fontSize = 48;
        fontWeight = 'bold';
      } else {
        switch (textStyle) {
          case 'heading':
            content = 'Add a heading';
            fontSize = 32;
            fontWeight = 'bold';
            break;
          case 'subheading':
            content = 'Add a subheading';
            fontSize = 24;
            fontWeight = 'bold';
            break;
          case 'body':
            content = 'Add a little bit of body text';
            fontSize = 16;
            fontWeight = 'normal';
            break;
          default:
            content = 'Text';
            fontSize = 16;
            fontWeight = 'normal';
        }
      }
    }

    const newElement: SlideElement = {
      id: `element-${Date.now()}`,
      type,
      x: 100,
      y: 100,
      width: type === 'text' ? (textStyle === 'heading' ? 300 : 200) : type === 'image' ? 150 : 100,
      height: type === 'text' ? (textStyle === 'heading' ? 60 : 50) : type === 'image' ? 150 : 100,
      content,
      isEditing: type === 'text',
      style: {
        fontSize,
        fontFamily: 'Arial',
        color: '#000000',
        fontWeight,
        textAlign: 'left',
        backgroundColor: type === 'shape' ? '#3b82f6' : undefined,
        borderRadius: type === 'shape' ? 0 : undefined,
      },
    };

    const newSlides = slides.map((slide, index) => {
      if (index === currentSlideIndex) {
        return {
          ...slide,
          elements: [...slide.elements, newElement],
        };
      }
      return slide;
    });

    set({ slides: newSlides, selectedElement: newElement.id });
    saveToHistory(newSlides);
  },

  updateElementStyle: (elementId, property, value) => {
    const { slides, currentSlideIndex, saveToHistory } = get();
    const newSlides = slides.map((slide, index) => {
      if (index === currentSlideIndex) {
        return {
          ...slide,
          elements: slide.elements.map((element) =>
            element.id === elementId
              ? {
                  ...element,
                  style: {
                    ...element.style,
                    [property]: value,
                  },
                }
              : element
          ),
        };
      }
      return slide;
    });
    set({ slides: newSlides });
    saveToHistory(newSlides);
  },

  deleteElement: (elementId) => {
    const { slides, currentSlideIndex, saveToHistory } = get();
    const newSlides = slides.map((slide, index) => {
      if (index === currentSlideIndex) {
        return {
          ...slide,
          elements: slide.elements.filter((el) => el.id !== elementId),
        };
      }
      return slide;
    });
    set({ slides: newSlides, selectedElement: null });
    saveToHistory(newSlides);
  },

  undo: () => {
    const { history, historyIndex } = get();
    if (historyIndex > 0) {
      set({
        historyIndex: historyIndex - 1,
        slides: history[historyIndex - 1],
      });
    }
  },

  redo: () => {
    const { history, historyIndex } = get();
    if (historyIndex < history.length - 1) {
      set({
        historyIndex: historyIndex + 1,
        slides: history[historyIndex + 1],
      });
    }
  },
}));

export default useSlideStore;