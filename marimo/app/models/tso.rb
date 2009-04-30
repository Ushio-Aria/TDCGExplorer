begin
  require "tahhash.so"
rescue LoadError
  #
end

class Tso < ActiveRecord::Base
  belongs_to :tah

  def before_save
    self.tah_hash = '%08X' % TAHHash.calc(path) if defined? TAHHash
    self.tah_hash ||= ''
    nil
  end

  def collisions_and_duplicates
    @_collisions_and_duplicates ||= self.class.find(:all, :conditions => ['tah_hash = ? and id <> ?', tah_hash, id])
  end

  def collisions
    collisions_and_duplicates.reject { |t| t.path.downcase == path.downcase }
  end

  def duplicates
    collisions_and_duplicates.select { |t| t.path.downcase == path.downcase }
  end

  class Search
    attr_accessor :path, :tah_hash

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
      self.path ||= ''
      self.tah_hash = '%08X' % TAHHash.calc(path) if defined? TAHHash
      self.tah_hash ||= ''
    end

    def collisions_and_duplicates
      @_collisions_and_duplicates ||= Tso.find(:all, :conditions => ['tah_hash = ?', tah_hash])
    end

    def collisions
      collisions_and_duplicates.reject { |t| t.path.downcase == path.downcase }
    end

    def duplicates
      collisions_and_duplicates.select { |t| t.path.downcase == path.downcase }
    end
  end
end
